using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DrBAE.TnM.WdmAnalyzer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DrBAE.WdmServer.WebApp.Models;
using DrBAE.TnM.Utility;
using System.Security.Claims;
using System.ComponentModel;
using System.Threading;
using System.Diagnostics;
using DrBAE.WdmServer.WebUtility;
using DrBAE.WdmServer.ExceptionProcessing;
using DrBAE.WdmServer.Models;
using Universe.DataAnalysis;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = AnalysisModel;
    using CM = ConfigModel;

    public class AnalysisController : AppControllerBase<AnalysisController, M>
    {
        readonly DbSet<AnalysisRawUpload> _dsUp;
        readonly DbSet<RawUpload> _dsRawUp;
        public AnalysisController(IServiceProvider sp) : base(sp, ConfigType.Analysis)
        {
            _dsUp = _dyContext.Set<AnalysisRawUpload>();
            _dsRawUp = _dyContext.Set<RawUpload>();
        }

        // GET: Analyses
        public async Task<IActionResult> Index()
        {
            logParams(nameof(Index));
            var analysis = await _ds.Include(a => a.Config).Include(a => a.AnalysisRawUploads).Include(a => a.AnalysisData).Include(a => a.User).ToListAsync();
            return View(analysis);
        }

        // GET: Analyses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var analysis = await _ds.Include(a => a.Config).Include(a => a.AnalysisRawUploads).Include(a => a.User).FirstOrDefaultAsync(x => x.Id == id);
            if (analysis == null) return NotFound();
            return View(analysis);
        }

        #region ---- Create ----

        // GET: Analyses/Create
        public IActionResult Create()
        {
            logParams(nameof(Create));
            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName);
            ViewData[M.pAnalysisRawUploads] = new SelectList(_dsRawUp, RawUpload.pId, RawUpload.pDesc);
            return View();
        }

        // POST: Analyses/Create
        [ValidateAntiForgeryToken, HttpPost, ActionName("Create")]
        [OptionalParameters(M.pUserId, M.pUser, M.pConfig, M.pAnalysisData, M.pAnalysisRawUploads)]
        public async Task<IActionResult> CreatePost(M model)
        {
            logParams(nameof(CreatePost), model);
            if (ModelState.IsValid)
            {
                using var tr = await _dyContext.Database.BeginTransactionAsync();
                model.Date = DateTime.UtcNow;
                model.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                model.Config = _configs.First(c => c.Id == model.ConfigId);
                _dyContext.Add(model);
                await _dyContext.SaveChangesAsync();

                //add items to AnlaysisRawUpload
                foreach (var rid in model.RawUploadIds)
                {
                    var au = new AnalysisRawUpload()
                    {
                        AnalysisId = model.Id,
                        RawUploadId = rid
                    };
                    _dsUp.Add(au);
                }
                await _dyContext.SaveChangesAsync();

                //check raw config integrity
                var ups = await _dsUp.Include(x => x.RawUpload).Include(x => x.RawUpload.RawData).Include(x => x.RawUpload.Config)
                    .Where(x => x.AnalysisId == model.Id).ToListAsync();
                var rawConfig = ups.First().RawUpload.Config;
                foreach (var up in ups) if (up.RawUpload.ConfigId != rawConfig.Id) ExBuilder.Create("inconsistent raw configs").Throw();

                //do anayses
                var watch = Stopwatch.StartNew();
                var failedList = runParallel(_sp, _dyContext, model, ups);
                model.NumDut = ups.Sum(u => u.RawUpload.NumDut) - failedList.Count;
                model.DeltaT = watch.ElapsedMilliseconds;
                //TODO: 모델에 failedList 기록

                await _dyContext.SaveChangesAsync();
                await tr.CommitAsync();

                //TODO: failedList를 출력

                return RedirectToAction(nameof(Index));
            }

            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName, model.ConfigId);
            return View(model);
        }

        List<RawDataModel> runParallel(IServiceProvider sp, DbContext context, M model, List<AnalysisRawUpload> ups)
        {
            var raws = new List<RawDataModel>();
            foreach (var u in ups.Select(x => x.RawUpload)) raws.AddRange(u.RawData);

            IRawLogic rawLogic = sp.LoadRawLogic(ups.First().RawUpload.Config.Content);

            var dut = (TypeOfDUT)Enum.Parse(typeof(TypeOfDUT), model.Config.Name);//TODO: ConfigType.Analysis인경우 ConfigModel.Name = TypeOfDUT.ToString()
            IAnalyzer analyzer = sp.LoadAnalyzer(dut);

            var failedList = new List<RawDataModel>();
            var lockObj = new object();
            Parallel.ForEach(raws, (rawDataModel) => analyseOne(rawDataModel, failedList, lockObj));
            return failedList;

            void analyseOne(RawDataModel rawDataModel, List<RawDataModel> failedList, object lockObj)
            {
                var raw = rawLogic.LoadFromBytes(rawDataModel.Data.Unzip(), rawDataModel.Sn); //저장된 RawData를 IRawData로 변환

                try
                {
                    var ad = new AnalysisDataModel();
                    ad.Data = analyzer.Analyze(raw).Pack();
                    ad.Sn = rawDataModel.Sn;
                    ad.AnalysisId = model.Id;
                    ad.RawDataId = rawDataModel.Id;
                    lock (lockObj) context.Add(ad);

                    var dad = new DutAnalysisModelBase();
                    dad.AnalysisData = ad;
                    lock (lockObj) context.Add(dad);
                }
                catch
                {
                    lock (lockObj) failedList.Add(rawDataModel);
                }
            }
        }
        

        #endregion 

        // GET: Analyses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            logParams(nameof(Edit), id);
            if (id == null) return NotFound();
            var analysis = await _ds.FindAsync(id);
            if (analysis == null) return NotFound();

            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName, analysis.ConfigId);
            return View(analysis);
        }

        // POST: Analyses/Edit/5
        [HttpPost, ActionName("Edit"), ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, M model)
        {
            logParams(nameof(EditPost), id, model);
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    await UpdateDbContextAsync(model, M.pDesc);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AnalysisExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName, model.ConfigId);
            return View(model);
        }

        // GET: Analyses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Index), id);
            if (id == null) return NotFound();
            var analysis = await _ds.Include(a => a.Config).FirstOrDefaultAsync(m => m.Id == id);
            if (analysis == null) return NotFound();
            return View(analysis);
        }

        // POST: Analyses/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(Index), id);
            var analysis = await _ds.FindAsync(id);
            _ds.Remove(analysis);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool AnalysisExists(int id) => _ds.Any(e => e.Id == id);
    }
}

