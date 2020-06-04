using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrBAE.WdmServer.WebApp.Data;
using DrBAE.WdmServer.WebApp.Models;
using System.IO;
using Microsoft.Extensions.Configuration;
using System.Text;
using System;
using Microsoft.Extensions.Logging;
using DrBAE.WdmServer.WebApp;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.AspNetCore.Hosting;
using Ko.Pigtail;
using DrBAE.WdmServer.WebUtility;
using DrBAE.WdmServer.Models;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = PigtailReport;
    using FM = PigtailReportFormat;

    public class PigtailReportController : AppControllerBase<PigtailReportController, M>
    {
        private readonly IPigtailLogic _PigtailLogic;
        private readonly DbSet<FM> _fm;
        private readonly DbSet<LotModel> _lot;

        public PigtailReportController(IPigtailLogic reportLogic, IServiceProvider sp) : base(sp)
        {
            _PigtailLogic = reportLogic;
            _fm = _dyContext.Set<FM>();
            _lot = _dyContext.Set<LotModel>();
        }

        // GET: PigtailReports
        public async Task<IActionResult> Index()
        {
            logParams(nameof(Index));
            var models = await _ds.Include(r => r.Lot).Include(r => r.Format).ToListAsync();
            return View(models);
        }

        // GET: PigtailReports/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var models = await _ds.Include(p => p.Lot).Include(p => p.Format).FirstOrDefaultAsync(m => m.Id == id);
            if (models == null) return NotFound();
            return View(models);
        }

        // GET: PigtailReports/Create
        public IActionResult Create()
        {
            logParams(nameof(Create));
            ViewData[M.pLotId] = new SelectList(_lot, LotModel.pId, LotModel.pName);
            ViewData[M.pFormatId] = new SelectList(_fm, FM.pId, FM.pName);
            return View();
        }

        // POST: PigtailReports/Create
        [ValidateAntiForgeryToken, HttpPost, ActionName("Create")]
        [OptionalParameters(M.pLot, M.pFormat, M.pDataFile, M.pReport, M.pReportName)]
        public async Task<IActionResult> CreatePost(M model)
        {
            logParams(nameof(CreatePost), model);

            await _dyContext.Set<AnalysisModel>().Include(a => a.AnalysisData).ToListAsync();//이게 왜 있지?

            if (ModelState.IsValid)
            {
                var format = await _fm.Include(x => x.Config).FirstOrDefaultAsync(x => x.Id == model.FormatId);
                var configText = format.Config?.Content;
                (model.DataFile, model.ReportName) = model.IDataFile.ToByte();
                //(model.ChipFile, model.ChipFileName) = model.IChipFile.ToByte();

                //TODO: Utility.dll 업데이트됨. 이것을 PigtailLogic에 반영 필요.
                var pigtailSerials = _PigtailLogic.PigtailSerial(configText, model.DataFile);//Pigtail에있는 시리얼 목록

                var chipDataList = _dyContext.Set<AnalysisDataModel>().Where(x => pigtailSerials.Contains(x.Sn)).Select(x => x.Data).ToList();

                //byte[] MakeReporter(string configText, byte[] formByte, List<string> chipString, byte[] pigtailByte, string lot);
                //ToDo:업로드된 pigtail 의 sn를 받아서 chipData목록을 db에서 뽑아서 텍스트로 만들어야함
                var lot = await _dyContext.FindAsync<LotModel>(model.LotId);
                var report = _PigtailLogic.MakeReporter(configText, format.FormFile, chipDataList, model.DataFile, lot.LotName);//Pigtail 성적서 model에 저장
                model.Report = report;

                _dyContext.Add(model);
                await _dyContext.SaveChangesAsync();

                ViewData[M.pLotId] = new SelectList(_lot, LotModel.pId, LotModel.pName, model.LotId);
                ViewData[M.pFormatId] = new SelectList(_fm, FM.pId, FM.pName, model.FormatId);
                return RedirectToAction("Index");
            }
            return View(model);
        }

        public async Task<IActionResult> Download(int? id, int? fileId)
        {
            logParams(nameof(Download), id, fileId);

            if (id == null) return NotFound();
            var fileModel = await _ds.Include(r => r.Lot).Include(r => r.Format).FirstOrDefaultAsync(m => m.Id == id);//Lot Navi 프로퍼티를 포함해서 가져옴
            if (fileModel == null) return NotFound();
            if (fileModel.Report == null) return RedirectToAction("Index");

            byte[] file;
            string fileName;

            if (fileId == 1)
            {
                file = fileModel.DataFile;
                fileName = $"{fileModel.ReportName}";
            }
            else
            {
                file = fileModel.Report;
                fileName = $"PigtailReport_{fileModel.Lot.LotName}.xlsx";
            }

            return File(file, "application/excel", fileName);
        }


        // GET: PigtailReports/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            logParams(nameof(Edit), id);
            if (id == null) return NotFound();

            var model = await _dyContext.FindAsync<M>(id);
            if (model == null) return NotFound();
            ViewData["LotId"] = new SelectList(_lot, LotModel.pId, LotModel.pName, model.LotId);
            return View(model);
        }

        // POST: PigtailReports/Edit/5
        [ValidateAntiForgeryToken, HttpPost, ActionName("Edit")]
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
                    if (!PigtailReportExists(model.Id)) return NotFound();
                    else throw;
                }

                return RedirectToAction(nameof(Index));
            }
            ViewData[M.pLotId] = new SelectList(_lot, LotModel.pId, LotModel.pName, model.LotId);
            ViewData[M.pFormatId] = new SelectList(_fm, FM.pId, FM.pName, model.FormatId);
            return View(model);
        }

        //[Authorize(Roles = "admin,delete")]
        // GET: PigtailReports/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Delete), id);
            if (id == null) return NotFound();
            var pigtailReport = await _ds.Include(p => p.Lot).Include(p => p.Format).FirstOrDefaultAsync(m => m.Id == id);
            if (pigtailReport == null) return NotFound();
            return View(pigtailReport);
        }

        // POST: PigtailReports/Delete/5
        //[Authorize(Roles = "admin,delete")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(DeleteConfirmed), id);
            var pigtailReport = await _ds.FindAsync(id);
            _dyContext.Remove(pigtailReport);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PigtailReportExists(int id) => _ds.Any(e => e.Id == id);
    }
}
