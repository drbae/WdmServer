using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Text;
using DrBAE.TnM.WdmAnalyzer;
using Microsoft.Extensions.Logging;
using DrBAE.WdmServer.WebUtility;
using Microsoft.AspNetCore.Http.Features;
using DrBAE.WdmServer.WebApp.Data;
using DrBAE.WdmServer.WebApp.Controllers;
using DrBAE.WdmServer.Models;
using Universe.DataAnalysis;
using Universe.Config;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = ConfigModel;
    public class ConfigModelController : AppControllerBase<ConfigModelController, M>
    {
        public ConfigModelController(IServiceProvider sp) : base(sp) { }

        // GET: Config
        public async Task<IActionResult> Index()
        {
            logParams(nameof(Index));
            return View(await _ds.ToListAsync());
        }

        // GET: Config/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var config = await _dyContext.FindAsync(_type, id);
            if (config == null) return NotFound();
            return View(config);
        }

        // GET: Config/Create
        public IActionResult Create()
        {
            logParams(nameof(Create));
            return View();
        }

        // POST: Config/Create
        [ValidateAntiForgeryToken, HttpPost, ActionName("Create"), OptionalParameters(M.pName, M.pContent)]
        public async Task<IActionResult> CreatePost(M model)
        {
            logParams(nameof(CreatePost), model);
            if (ModelState.IsValid)
            {
                using (var ms = new MemoryStream())
                {
                    model.File.CopyTo(ms);
                    model.Content = Encoding.UTF8.GetString(ms.ToArray());
                    model.Name = model.File.FileName;
                }

                await buildModel(_sp, _dyContext, model);

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        internal static async Task buildModel(IServiceProvider sp, DbContext context, M model)
        {
            if (model.ConfigType == ConfigType.Raw)
            {
                var cf = new ConfigFileBase().LoadFromText(model.Content);
                var config = new RawConfig();
                config.Load(cf);
                model.Pol = config.PolConfig != RawDataPolConfig.Avg;
                model.NumCh = config.ChList.Count;
            }
            else if (model.ConfigType == ConfigType.Analysis)
            {
                //var config = new AnalysisConfig();
                //model.Pol = config.IsMultiPol;
                //model.NumCh = config.NumChs;
            }
            else if (model.ConfigType == ConfigType.Pigtail) { }
            else if (model.ConfigType == ConfigType.Report) { }
            else throw new Exception($"Create() : 선택된 Type '{model.ConfigType}'에 대한 처리가 구현되지 않음");

            if(string.IsNullOrWhiteSpace(model.Description)) model.Description = $"{model.ConfigType} {model.NumCh} {model.Pol} ({model.Name})";
            context.Add(model);
            await context.SaveChangesAsync();
        }

        // GET: Config/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            logParams(nameof(Edit), id);
            if (id == null) return NotFound();
            var config = await _dyContext.FindAsync(_type, id);
            if (config == null) return NotFound();
            return View(config);
        }

        // POST: Config/Edit/5
        [ValidateAntiForgeryToken, HttpPost, ActionName("Edit"), OptionalParameters(M.pFile)]
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
                    if (!ConfigExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Config/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Delete), id);
            if (id == null) return NotFound();
            var config = await _dyContext.FindAsync(_type, id);
            if (config == null) return NotFound();
            return View(config);
        }

        // POST: Config/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(DeleteConfirmed), id);
            var config = _dyContext.Find(_type, id);
            _dyContext.Remove(config);
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ConfigExists(int id) => _dyContext.Find(_type, id) != null;
    }
}
