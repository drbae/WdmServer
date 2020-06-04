using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrBAE.WdmServer.WebApp.Data;
using Microsoft.AspNetCore.Http;
using DrBAE.WdmServer.WebApp;
using Microsoft.Extensions.Logging;
using DrBAE.WdmServer.WebUtility;
using DrBAE.WdmServer.Models;
using DrBAE.WdmServer.WebApp.Models;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = RawDataModel;
    public class RawDataModelController : AppControllerBase<RawDataModelController, M>
    {
        public RawDataModelController(IServiceProvider sp) : base(sp, ConfigType.Raw) { }

        // GET: RawDatas
        public async Task<IActionResult> Index(string? search)
        {
            logParams(nameof(Index));
            var models = await _ds.Include(a => a.RawUpload).Include(x => x.AnalysisDataModels).ToListAsync();
            //load analysis model
            foreach(var model in models) foreach (var ad in model.AnalysisDataModels)
                    await _dyContext.Set<AnalysisDataModel>().Include(x => x.Analysis).FirstAsync(x => x.Analysis.Id == ad.AnalysisId);

            return View(models);
        }

        // GET: RawDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var model = await _ds.Include(p => p.RawUpload.Config).Include(x => x.AnalysisDataModels).FirstOrDefaultAsync(m => m.Id == id);
            if (model == null) return NotFound();
            var rawLogic = _sp.LoadRawLogic(model.RawUpload.Config?.Content);

            //load analysis model
            foreach (var ad in model.AnalysisDataModels) 
                await _dyContext.Set<AnalysisDataModel>().Include(x => x.Analysis).FirstAsync(x => x.Analysis.Id == ad.AnalysisId);

            ViewData[M.pData] = rawLogic.ToText(model.Data);
            return View(model);
        }

        // GET: RawDatas/Create
        public IActionResult Create()
        {
            logParams(nameof(Create));
            return View();
        }

        // POST: RawDatas/Create
        [ValidateAntiForgeryToken, HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreatePost(M model)
        {
            logParams(nameof(CreatePost), model);
            if (ModelState.IsValid)
            {
                await _dyContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            await _dyContext.SaveChangesAsync();
            //ViewData["RawConfigId"] = new SelectList(_context.RawConfigModel, "Id", "Name", model.RawConfigId);
            return View(model);
        }

        // GET: RawDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            logParams(nameof(Edit), id);
            if (id == null) return NotFound();
            var rawData = await _ds.FindAsync(id);
            if (rawData == null) return NotFound();
            return View(rawData);
        }

        // POST: RawDatas/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken, HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id, M model)
        {
            logParams(nameof(EditPost), id, model);
            if (id != model.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    (model.Data, model.Sn) = model.File.ToByte();
                    _dyContext.Update(model);
                    await _dyContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RawDataExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: RawDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Delete), id);
            if (id == null) return NotFound();
            var rawData = await _ds.FirstOrDefaultAsync(m => m.Id == id);
            if (rawData == null) return NotFound();
            return View(rawData);
        }

        // POST: RawDatas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(DeleteConfirmed), id);
            var rawData = await _ds.FindAsync(id);
            _ds.Remove(rawData);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool RawDataExists(int id)
        {
            return _ds.Any(e => e.Id == id);
        }

        public async Task<IActionResult> Download(int? id)
        {
            logParams(nameof(Download), id);
            if (id == null) return NotFound();
            var model = await _ds.Include(r => r.RawUpload).Include(r => r.RawUpload.Config).FirstOrDefaultAsync(m => m.Id == id);
            if (model == null) return NotFound();

            var rawLogic = _sp.LoadRawLogic(model.RawUpload.Config?.Content);
            string file = rawLogic.ToText(model.Data);
            string fileName = $"{model.Sn}-{id}.txt";
            return File(Encoding.ASCII.GetBytes(file), "text/plain", fileName);
        }
    }
}
