using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using DrBAE.WdmServer.WebApp.Data;
using DrBAE.WdmServer.WebApp.Models;
using DrBAE.WdmServer.WebApp;
using DrBAE.WdmServer.WebUtility;
using DrBAE.WdmServer.Models;

namespace DrBAE.WdmServer.WebApp.Controllers
{
    using M = LotModel;
    public class LotModelController : AppControllerBase<LotModelController, M>
    {
        public LotModelController(IServiceProvider sp) : base(sp) { }

        // GET: Lots
        public async Task<IActionResult> Index()
        {
            logParams(nameof(Index));
            return View(await _ds.ToListAsync());
        }

        // GET: Lots/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            logParams(nameof(Details), id);
            if (id == null) return NotFound();
            var lot = await _dyContext.FindAsync(_type, id);
            if (lot == null) return NotFound();
            return View(lot);
        }

        // GET: Lots/Create
        public IActionResult Create()
        {
            return View(new M());
        }

        // POST: Lots/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [ValidateAntiForgeryToken, HttpPost, ActionName("Create")]
        public async Task<IActionResult> CreatePost(M model)
        {
            logParams(nameof(CreatePost), model);
            if (ModelState.IsValid)
            {
                _dyContext.Add(model);
                await _dyContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Lots/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            logParams(nameof(Edit), id);
            if (id == null) return NotFound();
            var lot = await _dyContext.FindAsync(_type, id);
            if (lot == null) return NotFound();
            return View(lot);
        }

        // POST: Lots/Edit/5
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
                    await UpdateDbContextAsync(model, M.pName);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LotExists(model.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Lots/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            logParams(nameof(Delete), id);
            if (id == null) return NotFound();
            var lot = await _dyContext.FindAsync(_type, id);
            if (lot == null) return NotFound();
            return View(lot);
        }

        // POST: Lots/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            logParams(nameof(DeleteConfirmed), id);
            _dyContext.Remove(_dyContext.Find(_type, id));
            await _dyContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LotExists(int id) => _dyContext.Find(_type, id) != null;
    }
}
