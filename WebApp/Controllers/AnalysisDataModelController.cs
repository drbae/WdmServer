using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrBAE.WdmServer.WebApp.Data;
using DrBAE.WdmServer.WebApp.Models;
using Microsoft.Extensions.Logging;
using DrBAE.WdmServer.WebUtility;
using DrBAE.WdmServer.ExceptionProcessing;
using DrBAE.WdmServer.Models;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = AnalysisDataModel;

    public class AnalysisDataModelController : AppControllerBase<AnalysisDataModelController, M>
    {
        public AnalysisDataModelController(IServiceProvider sp) : base(sp) { }

        // GET: AnalysisData
        public async Task<IActionResult> Index(string? search)
        {
            logParams(nameof(Index));
            var models = await _ds.Include(a => a.Analysis).Include(x => x.RawData).ToListAsync();
            if (search != null) return View(models.Where(x => x.Analysis.Description.Contains(search)));
            else return View(models);
        }

        // GET: AnalysisData/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var model = await _ds.Include(a => a.Analysis).Include(x => x.RawData).FirstOrDefaultAsync(x => x.Id == id);
            if (model == null) return NotFound();
            model.Data = model.Data.Replace("\n", "\n\n");
            model.Data = model.Data.Replace(' ', '\n');
            model.Data = model.Data.Replace('|', '\n');
            return View(model);
        }

        // GET: AnalysisData/Create
        public IActionResult Create()
        {
            logParams(nameof(Index));
            return View();
        }

        // POST: AnalysisData/Create
        [ValidateAntiForgeryToken, HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreatePost(M analysisData)
        {
            logParams(nameof(CreatePost), analysisData);
            if (ModelState.IsValid)
            {
                _dyContext.Add(analysisData);
                await _dyContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(analysisData);
        }

        // GET: AnalysisData/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Delete), id);
            if (id == null) return NotFound();
            var analysisData = await _ds.FindAsync(id);
            if (analysisData == null) return NotFound();
            return View(analysisData);
        }

        // POST: AnalysisData/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(DeleteConfirmed), id);
            var analysisData = await _ds.FindAsync(id);
            _ds.Remove(analysisData);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AnalysisDataExists(int id) => _ds.Any(e => e.Id == id);
    }
}

