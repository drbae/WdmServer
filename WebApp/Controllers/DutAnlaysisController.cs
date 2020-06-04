using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DrBAE.WdmServer.WebApp.Data;
using DrBAE.TnM.WdmAnalyzer;
using DrBAE.WdmServer.Models;
using Universe.DataAnalysis;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = DutAnalysisModelBase;

    public class DutAnlaysisController : AppControllerBase<DutAnlaysisController, M>
    {
        readonly IReportLogic _report;
        public DutAnlaysisController(IServiceProvider sp) : base(sp, ConfigType.Report)
        {
            //TODO: 어떤 report config를 쓸지 ~ UI 지정?
            var reportConfig = _configs.Where(x => x.Name.Contains("CH_Sum")).First();
            _report = _sp.LoadReportLogic(reportConfig.Content);
        }
     
        // GET: ChipDatas
        public async Task<IActionResult> Index()
        {
            //TODO: 각 레코드의 AnalysisConfig 가 다를 경우 처리?

            var models = await _ds.Include(x => x.AnalysisData).Include(x => x.AnalysisData.Analysis).Include(x => x.AnalysisData.Analysis.Config).ToListAsync();
            ViewData["ColumnHeaders"] = models.Count > 0 ? getCols(models[0]) : new List<(int, object)>();
            ViewData["vms"] = models.ToDictionary(m => m.Id, m => getVm(m));

            return View(models);

            List<object> getVm(M model)//get view model
            {
                var ad = new AnalysisData(model.AnalysisData.Data);
                //var ad = _sp.LoadAnalyzer(model.AnalysisData.Analysis.Config.Content).Load(model.AnalysisData.Data);//new AnalysisData(packedAnalsysData)
                _report.AddAnalysis(ad);
                return _report.Specs.Select(p => p.Value.Item2).ToList();
            }
            List<(int, object)> getCols(M model)
            {
                var colCounter = 0;
                var ad = new AnalysisData(model.AnalysisData.Data);
                _report.AddAnalysis(ad);
                //foreach (var h in _report.Specs.Keys) cols.Add((colCounter++, $"{h.Item1}\n{h.Item2}"));
                return _report.Specs.Keys.Select(h => (colCounter++, (object)$"{h.Item1}\n{h.Item2}")).ToList();
            }
        }
        

        // GET: ChipDatas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //IAnalsysData
            if (id == null) return NotFound();
            var model = await _ds.Include(x => x.AnalysisData).Include(x => x.AnalysisData.Analysis).Include(x => x.AnalysisData.Analysis.Config)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (model == null) return NotFound();
            
            var ad = new AnalysisData(model.AnalysisData.Data);//Func<string, IAnalysisData>
            _report.AddAnalysis(ad);
            var vms = _report.Specs.Select(p => (ValueTuple<object, object>)(p.Key, p.Value.Item2)).ToList();

            ViewData["vms"] = vms;

            return View(model);
        }

        // GET: ChipDatas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: ChipDatas/Create
        [ValidateAntiForgeryToken, HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreatePost(M model)
        {
            if (ModelState.IsValid)
            {
                _dyContext.Add(model);
                await _dyContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: ChipDatas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var chipData = await _ds.FindAsync(id);
            if (chipData == null) return NotFound();
            return View(chipData);
        }

        // POST: ChipDatas/Edit/5
        [ValidateAntiForgeryToken, HttpPost, ActionName("Edit")]
        public async Task<IActionResult> EditPost(int id, M chipData)
        {
            if (id != chipData.Id) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _dyContext.Update(chipData);
                    await _dyContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ChipDataExists(chipData.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(chipData);
        }

        // GET: ChipDatas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var chipData = await _ds.FirstOrDefaultAsync(m => m.Id == id);
            if (chipData == null) return NotFound();
            return View(chipData);
        }

        // POST: ChipDatas/Delete/5
        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var chipData = await _ds.FindAsync(id);
            _ds.Remove(chipData);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ChipDataExists(int id) => _ds.Any(e => e.Id == id);
    }
}
