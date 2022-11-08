using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ModelsView;

namespace Pharmacy.Controllers
{
    [Authorize]
    public class HealthcareSystemsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HealthcareSystemsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [AllowAnonymous]
        // GET: HealthcareSystems
        public async Task<IActionResult> Index(int page = 1)
        {
            pager pager = new pager()
            {
                currentPage = page,
                rowXpag = 5
            };

            var query = _context.healthcareSystem.Select(a => a);

            pager.rows = query.Count();

            var showInformation = query
               .Skip((pager.currentPage - 1) * pager.rowXpag)
               .Take(pager.rowXpag);

            foreach (var item in Request.Query)
                pager.ValuesQueryString.Add(item.Key, item.Value);

            HealthcareSystemViewModel Information = new HealthcareSystemViewModel()
            {
                HealthcareSystemList = showInformation.ToList(),
                pager = pager
            };

            return View(Information);
        }

        [AllowAnonymous]
        // GET: HealthcareSystems/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthcareSystem = await _context.healthcareSystem
                .FirstOrDefaultAsync(m => m.id == id);
            if (healthcareSystem == null)
            {
                return NotFound();
            }

            return View(healthcareSystem);
        }

        // GET: HealthcareSystems/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: HealthcareSystems/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name")] HealthcareSystem healthcareSystem)
        {
            if (ModelState.IsValid)
            {
                _context.Add(healthcareSystem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(healthcareSystem);
        }

        // GET: HealthcareSystems/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthcareSystem = await _context.healthcareSystem.FindAsync(id);
            if (healthcareSystem == null)
            {
                return NotFound();
            }
            return View(healthcareSystem);
        }

        // POST: HealthcareSystems/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name")] HealthcareSystem healthcareSystem)
        {
            if (id != healthcareSystem.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(healthcareSystem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HealthcareSystemExists(healthcareSystem.id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(healthcareSystem);
        }

        // GET: HealthcareSystems/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var healthcareSystem = await _context.healthcareSystem
                .FirstOrDefaultAsync(m => m.id == id);
            if (healthcareSystem == null)
            {
                return NotFound();
            }

            return View(healthcareSystem);
        }

        // POST: HealthcareSystems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var healthcareSystem = await _context.healthcareSystem.FindAsync(id);
            if (_context.customer.Where(a => a.HealthcareSystemid == id).Count() == 0)
            {
                _context.healthcareSystem.Remove(healthcareSystem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            else
            {
                ViewData["error"] = "Existen datos relacionados a la obra social, por lo que no puede eliminarse";
                return View(healthcareSystem);
            }
        }

        private bool HealthcareSystemExists(int id)
        {
            return _context.healthcareSystem.Any(e => e.id == id);
        }
    }
}
