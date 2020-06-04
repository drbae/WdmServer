using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using DrBAE.TnM.Utility;
using System.Security.Claims;
using System.Diagnostics;
using Microsoft.AspNetCore.Http;
using DrBAE.WdmServer.WebUtility;
using DrBAE.WdmServer.Models;
using DrBAE.WdmServer.ExceptionProcessing;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = RawUpload;
    using DM = RawDataModel;
    using CM = ConfigModel;
    public class RawUploadController : AppControllerBase<RawUploadController, M>
    {
        public RawUploadController(IServiceProvider sp) : base(sp, ConfigType.Raw) { }

        // GET: RawUpload
        public async Task<IActionResult> Index()
        {
            logParams(nameof(Index));
            var models = await _ds.Include(x => x.Config).Include(x => x.RawData).Include(x => x.AnalysisRawUploads).Include(x => x.User).ToListAsync();
            return View(models);
        }

        // GET: RawUpload/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var model = await _ds.Include(r => r.Config).Include(r => r.RawData).Include(x => x.AnalysisRawUploads).Include(r => r.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (model == null) return NotFound();

            //load analysis model
            foreach (var ad in model.AnalysisRawUploads)
                await _dyContext.Set<AnalysisRawUpload>().Include(x => x.Analysis).FirstAsync(x => x.Analysis.Id == ad.AnalysisId);

            return View(model);
        }

        // GET: RawUpload/Create
        public IActionResult Create()
        {
            logParams(nameof(Create));
            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName).ToList();
            return View();
        }

        // POST: RawUpload/Create
        [HttpPost, ActionName("Create"), ValidateAntiForgeryToken]//TODO: ValidateAntiForgeryToken??
        [OptionalParameters(M.pUserId, M.pUser, M.pConfig, M.pRawData, M.pAnalysisRawUploads)]
        public async Task<IActionResult> CreatePost(M model)
        {
            if (ModelState.IsValid)
            {
                model.UserId = User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? model.UserId;
#if DEBUG
                if (string.IsNullOrWhiteSpace(model.UserId)) ExBuilder.Create("Can't find user id").Throw();
#endif

                var config = _configs.First(x => x.Id == model.ConfigId);
                await buildModel(_sp, _dyContext, model, config);

                await _dyContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName, model.ConfigId);
            return View(model);
        }
        internal static async Task buildModel(IServiceProvider sp, DbContext context, M model, CM config)
        {
            using var transaction = await context.Database.BeginTransactionAsync();
            //model set 
            model.Date = DateTime.UtcNow;
            var logic = sp.LoadRawLogic(config.Content);
            model.RawLogicVersion = logic.GetVersion();
            context.Add(model);
            await context.SaveChangesAsync();

            //
            var watch = Stopwatch.StartNew();
            List<(string Name, byte[] Value)> fileList;

            if (model.ZipFile != null) fileList = Compression.UnzipList(model.ZipFile);//zip upload test
            else if (model.Files != null)
            {
                fileList = _toList(model.Files);//현재 첫 파일이 zip 이면 그 파일 하나면 처리함
                if (Path.GetExtension(fileList[0].Name) == ".zip") fileList = Compression.UnzipList(fileList[0].Value);
            }
            else throw new Exception();

            var failedList = new List<string>();
            object lockObj = new object();

            Parallel.ForEach(fileList, (file) =>
            {
                var fileExt = Path.GetExtension(file.Name);
                var datum = new DM();
                if (fileExt == ".txt")
                {
                    try { _addDb(datum, file.Name, file.Value); }
                    catch { lock (lockObj) failedList.Add(file.Name); }
                }
                else //.txt가 아니면 에러 처리
                {
                    datum.Sn = file.Name;
                    lock (lockObj) failedList.Add(file.Name);
                }
            });
            model.NumDut = fileList.Count - failedList.Count;
            model.DeltaT = watch.ElapsedMilliseconds;
            context.Update(model);
            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            List<(string Name, byte[] Value)> _toList(List<IFormFile> formFiles)
            {
                var list = new List<(string Name, byte[] Value)>(formFiles.Count);
                Parallel.ForEach(formFiles, (formFile) => list.Add(_toByte(formFile)));
                return list;
            }
            (string Name, byte[] Value) _toByte(IFormFile formFile)
            {
                var byteArray = new byte[formFile.Length];
                using (var ms = new MemoryStream(byteArray)) formFile.CopyTo(ms);
                return (formFile.FileName, byteArray);
            }
            void _addDb(DM datum, string sn, byte[] data)
            {
                var rawData = logic?.LoadFromBytes(data, sn) ?? throw new NullReferenceException();
                datum.Data = logic.ToBytes(rawData);
                datum.RawUploadId = model.Id;
                datum.Sn = rawData.SN;
                lock (lockObj) context.Add(datum);
            }
        }

        // GET: RawUpload/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            logParams(nameof(Edit), id);
            if (id == null) return NotFound();
            var rawUpload = await _ds.FindAsync(id);
            if (rawUpload == null) return NotFound();
            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName, rawUpload.ConfigId);
            return View(rawUpload);
        }

        // POST: RawUpload/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken, HttpPost, ActionName("Edit")]
        [OptionalParameters(M.pUserId, M.pUser, M.pConfig, M.pRawData)]
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
                    if (!RawUploadExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }


            ViewData[M.pConfigId] = new SelectList(_configs, CM.pId, CM.pName, model.ConfigId);
            return View(model);
        }

        // GET: RawUpload/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Delete), id);
            if (id == null) return NotFound();
            var rawUpload = await _ds.Include(r => r.Config).Include(r => r.RawData).FirstOrDefaultAsync(m => m.Id == id);
            if (rawUpload == null) return NotFound();
            return View(rawUpload);
        }

        // POST: RawUpload/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(DeleteConfirmed), id);
            var raws = await _ds.Include(r => r.RawData).FirstOrDefaultAsync(m => m.Id == id);
            _ds.Remove(raws);
            if(raws.RawData != null) foreach (var rawData in raws.RawData) _dyContext.Set<DM>().Remove(rawData);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RawUploadExists(int id) => _ds.Any(e => e.Id == id);

        public async Task<IActionResult> Download(int? id)
        {
            logParams(nameof(Download), id);
            if (id == null) return NotFound();
            var model = await _ds.Include(r => r.RawData).Include(r => r.Config).FirstOrDefaultAsync(m => m.Id == id);
            if (model == null) return NotFound();
            var rawLogic = _sp.LoadRawLogic(model.Config?.Content);

            var files = new List<(string Name, string Context)>();
            if(model.RawData != null) foreach (var raw in model.RawData) files.Add(($"{raw.Sn}.txt", rawLogic.ToText(raw.Data)));
            var zipFile = Compression.Zip(files);
            return File(zipFile, "application/zip", $"{model.Description}_{model.Date}.zip");
        }
    }
}
