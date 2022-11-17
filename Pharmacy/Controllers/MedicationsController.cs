using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pharmacy.Data;
using Pharmacy.Models;
using Pharmacy.ModelsView;

namespace Pharmacy.Controllers
{
    [Authorize]
    public class MedicationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;

        public MedicationsController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        [AllowAnonymous]
        public FileResult Export()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Nombre; Precio; Laboratorio; Categoría;\r\n");
            foreach (Medication medication in _context.medication.Include(a => a.lab).Include(a => a.category).ToList())
            {
                sb.Append(medication.id + ";");
                sb.Append(medication.name + ";");
                sb.Append(medication.price + ";");
                sb.Append(medication.Labid + ";");
                sb.Append(medication.lab.name + ";");
                sb.Append(medication.Categoryid + ";");
                sb.Append(medication.category.description + ";");
                //Append new line character.
                sb.Append("\r\n");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "listadoMedicamentos.csv");
        }

        public IActionResult Import()
        {
            var files = HttpContext.Request.Form.Files;
            if (files != null && files.Count > 0)
            {
                var fileImports = files[0];
                var pathDestiny = Path.Combine(env.WebRootPath, "imports");
                if (fileImports.Length > 0)
                {
                    var fileDestiny = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(fileImports.FileName);
                    string route = Path.Combine(pathDestiny, fileDestiny);
                    using (var filestream = new FileStream(route, FileMode.Create))
                    {
                        fileImports.CopyTo(filestream);
                    };
  
                    using (var file = new FileStream(route, FileMode.Open))
                    {
                        List<string> rows = new List<string>();
                        List<Medication> MedicationFile = new List<Medication>();

                        StreamReader fileContent = new StreamReader(file); // System.Text.Encoding.Default
                        do
                        {
                            rows.Add(fileContent.ReadLine());
                        }
                        while (!fileContent.EndOfStream);

                        foreach (string row in rows)
                        {
                            int output;
                            string[] information = row.Split(';');
                            int lab = int.TryParse(information[information.Length - 1], out output) ? output : 0;
                            if (lab > 0 && _context.lab.Where(c => c.id == lab).FirstOrDefault() != null)
                            {
                                int category = int.TryParse(information[information.Length - 1], out output) ? output : 0;
                                if (category > 0 && _context.category.Where(c => c.id == category).FirstOrDefault() != null)
                                {
                                    Medication temporalMedication = new Medication()
                                    {
                                        Labid = lab,
                                        Categoryid = category,
                                        name = information[0],
                                        price = int.TryParse(information[1], out output) ? output : 0,
                                    };

                                    MedicationFile.Add(temporalMedication);
                                }
                            }
                        }
                        if (MedicationFile.Count > 0)
                        {
                            _context.medication.AddRange(MedicationFile);
                            _context.SaveChanges();
                        }

                        ViewBag.amountrows = MedicationFile.Count + " de " + rows.Count;
                    }
                }
            }

            return View();
        }

        [AllowAnonymous]
        // GET: Medications
        public async Task<IActionResult> Index(string findName, int? labId, int? categoryId, int page = 1)
        {
            pager pager = new pager()
            {
                currentPage = page,
                rowXpag = 3
            };

            var query = _context.medication.Include(a => a.lab).Include(a => a.category).Select(a => a);
            if (!string.IsNullOrEmpty(findName))
            {
                query = query.Where(e => e.name.Contains(findName));
                //paginador.ValoresQueryString.Add("busqNombre", busqNombre);
            }
            if (labId.HasValue)
            {
                query = query.Where(e => e.Labid == labId);
                //paginador.ValoresQueryString.Add("carreraId", carreraId.ToString());
            }
            if (categoryId.HasValue)
            {
                query = query.Where(e => e.Categoryid == categoryId);
                //paginador.ValoresQueryString.Add("carreraId", carreraId.ToString());
            }

            pager.rows = query.Count();

            //paginador.totalPag = (int)Math.Ceiling((decimal)paginador.cantReg / paginador.regXpag);
            var showInformation = query
                .Skip((pager.currentPage - 1) * pager.rowXpag)
                .Take(pager.rowXpag);

            foreach (var item in Request.Query)
                pager.ValuesQueryString.Add(item.Key, item.Value);

            MedicationViewModel information = new MedicationViewModel()
            {
                MedicationList = showInformation.ToList(),
                LabList = new SelectList(_context.lab, "id", "name", labId),
                CategoryList = new SelectList(_context.category, "id", "description", categoryId),
                findName = findName,
                pager = pager
            };

            return View(information);
        }

        [AllowAnonymous]
        // GET: Medications/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.medication
                .Include(m => m.lab)
                .Include(m => m.category)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // GET: Medications/Create
        public IActionResult Create()
        {
            ViewData["Labid"] = new SelectList(_context.lab, "id", "name");
            ViewData["Categoryid"] = new SelectList(_context.category, "id", "description");
            return View();
        }

        // POST: Medications/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,name,price,Categoryid,Labid,photo")] Medication medication)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files != null && files.Count > 0)
                {
                    var filePhoto = files[0];
                    var pathDestiny = Path.Combine(env.WebRootPath, "photos");
                    if (filePhoto.Length > 0)
                    {
                        var fileDestiny = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(filePhoto.FileName);

                        using (var filestream = new FileStream(Path.Combine(pathDestiny, fileDestiny), FileMode.Create))
                        {
                            filePhoto.CopyTo(filestream);
                            medication.photo = fileDestiny;
                        };

                    }
                }
                _context.Add(medication);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Labid"] = new SelectList(_context.lab, "id", "name", medication.Labid);
            ViewData["Categoryid"] = new SelectList(_context.category, "id", "description", medication.Categoryid);
            return View(medication);
        }

        // GET: Medications/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.medication.FindAsync(id);
            if (medication == null)
            {
                return NotFound();
            }
            ViewData["Labid"] = new SelectList(_context.lab, "id", "name", medication.Labid);
            ViewData["Categoryid"] = new SelectList(_context.category, "id", "description", medication.Categoryid);
            return View(medication);
        }

        // POST: Medications/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,name,price,Categoryid,Labid,photo")] Medication medication)
        {
            if (id != medication.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var files = HttpContext.Request.Form.Files;
                    if (files != null && files.Count > 0)
                    {
                        var filePhoto = files[0];
                        var pathDestiny = Path.Combine(env.WebRootPath, "photos");
                        if (filePhoto.Length > 0)
                        {
                            var fileDestiny = Guid.NewGuid().ToString().Replace("-", "") + Path.GetExtension(filePhoto.FileName);

                            if (!string.IsNullOrEmpty(medication.photo))
                            {
                                string photoBefore = Path.Combine(pathDestiny, medication.photo);
                                if (System.IO.File.Exists(photoBefore))
                                    System.IO.File.Delete(photoBefore);
                            }

                            using (var filestream = new FileStream(Path.Combine(pathDestiny, fileDestiny), FileMode.Create))
                            {
                                filePhoto.CopyTo(filestream);
                                medication.photo = fileDestiny;
                            };
                        }
                    }
                    _context.Update(medication);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationExists(medication.id))
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
            ViewData["Labid"] = new SelectList(_context.lab, "id", "name", medication.Labid);
            ViewData["Categoryid"] = new SelectList(_context.category, "id", "description", medication.Categoryid);
            return View(medication);
        }

        // GET: Medications/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medication = await _context.medication
                .Include(m => m.lab)
                .Include(m => m.category)
                .FirstOrDefaultAsync(m => m.id == id);
            if (medication == null)
            {
                return NotFound();
            }

            return View(medication);
        }

        // POST: Medications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medication = await _context.medication.FindAsync(id);
            _context.medication.Remove(medication);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationExists(int id)
        {
            return _context.medication.Any(e => e.id == id);
        }
    }
}