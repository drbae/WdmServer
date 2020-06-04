using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrBAE.WdmServer.Models;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = PigtailReportFormat;
    public class PigtailReportFormatController : AppControllerBase<PigtailReportFormatController, M>
    {
        public PigtailReportFormatController(IServiceProvider sp) : base(sp, ConfigType.Pigtail) { }

        // GET: Duts
        public async Task<IActionResult> Index()
        {
            logParams(nameof(Index));
            var appDbContext = _ds.Include(r => r.Config);
            return View(await appDbContext.ToListAsync());
        }

        // GET: Duts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var PigtailReportFormat = await _dyContext.FindAsync(_type, id);
            if (PigtailReportFormat == null) return NotFound();
            return View(PigtailReportFormat);
        }

        // GET: Duts/Create
        public IActionResult Create()
        {
            logParams(nameof(Create));
            ViewData[M.pConfigId] = new SelectList(_configs, ConfigModel.pId, ConfigModel.pName).ToList();
            return View();
        }
        // POST: Duts/Create
        [HttpPost, ActionName("Create")]
        [ValidateAntiForgeryToken]
        [OptionalParameters(M.pFormFile, M.pConfig)]
        public async Task<IActionResult> CreatePost(M model)
        {
            logParams(nameof(CreatePost), model);
            if (ModelState.IsValid && model.IFormFile != null)
            {
                using (var ms = new MemoryStream())
                {
                    model.IFormFile.CopyTo(ms);
                    model.FormFile = ms.ToArray();
                }
                _dyContext.Add(model);
                await _dyContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Duts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            logParams(nameof(Edit), id);
            if (id == null) return NotFound();
            var PigtailReportFormat = await _dyContext.FindAsync(_type, id);
            if (PigtailReportFormat == null) return NotFound();
            return View(PigtailReportFormat);
        }

        // POST: Duts/Edit/5
        [ValidateAntiForgeryToken, HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id, M model)
        {
            logParams(nameof(EditPost), id, model);
            _dyContext.Entry(model).State = EntityState.Modified;
            if (ModelState.IsValid)
            {
                try
                {
                    await UpdateDbContextAsync(model, M.pName);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PigtailReportFormatExists(id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Duts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Delete), id);
            if (id == null) return NotFound();
            var model = await _ds.Where(x => x.Id == id).Include(r => r.Config).FirstOrDefaultAsync();
            if (model == null) return NotFound();
            return View(model);
        }

        // POST: Duts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(DeleteConfirmed), id);
            var model = await _dyContext.FindAsync(_type, id);
            _dyContext.Remove(model);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PigtailReportFormatExists(int id) => _dyContext.Find(_type, id) != null;

        public async Task<IActionResult> DownloadFormFile(int? id)
        {
            logParams(nameof(DownloadFormFile), id);
            if (id == null) return NotFound();
            var model = await _dyContext.FindAsync<M>(id);
            if (model == null) return NotFound();
            if (model.FormFile == null) return RedirectToAction("Index");
            return File(model.FormFile, "application/excel", $"{model.Name}_FormFile.xlsx");
        }

        public async Task<IActionResult> DownloadConfigFile(int? id)
        {
            logParams(nameof(DownloadConfigFile), id);
            if (id == null) return NotFound();
            var model = await _dyContext.FindAsync<M>(id);
            if (model == null) return NotFound();
            return File(Encoding.UTF8.GetBytes(model.Config?.Content ?? "unknown error"), "application/excel", $"{model.Name}_ConfigFile.ini");
        }
    }
}
