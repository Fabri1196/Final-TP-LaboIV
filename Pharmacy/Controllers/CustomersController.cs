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
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment env;


        public CustomersController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            this.env = env;
        }

        [AllowAnonymous]
        public FileResult Export()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Apellido; Nombre; Dirección; Obra Social;\r\n");
            foreach (Customer customer in _context.customer.Include(a => a.healthcareSystem).ToList())
            {
                sb.Append(customer.id + ";");
                sb.Append(customer.surname + ";");
                sb.Append(customer.name + ";");
                sb.Append(customer.address + ";");
                sb.Append(customer.HealthcareSystemid + ";");
                sb.Append(customer.healthcareSystem.name + ";");
                //Append new line character.
                sb.Append("\r\n");
            }

            return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "listadoClientes.csv");
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
                        List<Customer> CustomerFile = new List<Customer>();

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
                            int healthcareSystem = int.TryParse(information[information.Length - 1], out output) ? output : 0;
                            if (healthcareSystem > 0 && _context.healthcareSystem.Where(c => c.id == healthcareSystem).FirstOrDefault() != null)
                            {
                                Customer temporalCustomer = new Customer()
                                {
                                    HealthcareSystemid = healthcareSystem,
                                    surname = information[0],
                                    name = information[1],
                                    address = information[2],
                                };
                                CustomerFile.Add(temporalCustomer);
                            }
                        }
                        if (CustomerFile.Count > 0)
                        {
                            _context.customer.AddRange(CustomerFile);
                            _context.SaveChanges();
                        }

                        ViewBag.amountrows = CustomerFile.Count + " de " + rows.Count;
                    }
                }
            }

            return View();
        }

        [AllowAnonymous]
        // GET: Customers
        public async Task<IActionResult> Index(string findSurname, string findName, int? healthcareSystemId, int page = 1)
        {
            pager pager = new pager()
            {
                currentPage = page,
                rowXpag = 3
            };

            var query = _context.customer.Include(a => a.healthcareSystem).Include(p => p.CustomerMedication).ThenInclude(sp => sp.medication).Select(a => a);
            if (!string.IsNullOrEmpty(findSurname))
            {
                query = query.Where(e => e.surname.Contains(findSurname));
            }
            if (!string.IsNullOrEmpty(findName))
            {
                query = query.Where(e => e.name.Contains(findName));
            }
            if (healthcareSystemId.HasValue)
            {
                query = query.Where(e => e.HealthcareSystemid == healthcareSystemId);
            }

            pager.rows = query.Count();

            var showInformation = query
                .Skip((pager.currentPage - 1) * pager.rowXpag)
                .Take(pager.rowXpag);

            foreach (var item in Request.Query)
                pager.ValuesQueryString.Add(item.Key, item.Value);

            CustomerViewModel information = new CustomerViewModel()
            {
                CustomerList = showInformation.ToList(),
                HealthcareSystemList = new SelectList(_context.healthcareSystem, "id", "name", healthcareSystemId),
                findSurname = findSurname,
                findName = findName,
                pager = pager
            };

            return View(information);
        }

        [AllowAnonymous]
        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.customer
                .Include(c => c.healthcareSystem)
                .Include(p => p.CustomerMedication).ThenInclude(sp => sp.medication)
                .FirstOrDefaultAsync(m => m.id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewData["HealthcareSystemid"] = new SelectList(_context.healthcareSystem, "id", "name");
            MedicationDropDownList();
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("id,surname,name,photo,address,HealthcareSystemid")] Customer customer, string[] selectedMedication)
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
                            customer.photo = fileDestiny;
                        };

                    }
                }
                CreateCustomerMedication(selectedMedication, customer);
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["HealthcareSystemid"] = new SelectList(_context.healthcareSystem, "id", "name", customer.HealthcareSystemid);
            return View(customer);
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.customer
                .Include(p => p.CustomerMedication).ThenInclude(sp => sp.medication)
                .FirstOrDefaultAsync(p => p.id == id);
            if (customer == null)
            {
                return NotFound();
            }
            MedicationDropDownList(customer);
            ViewData["HealthcareSystemid"] = new SelectList(_context.healthcareSystem, "id", "name", customer.HealthcareSystemid);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("id,surname,name,photo,address,HealthcareSystemid")] Customer customer, string[] selectedMedication)
        {
            var exCustomer = await _context.customer
                .Where(p => p.id == customer.id)
                .Include(p => p.CustomerMedication).ThenInclude(sp => sp.medication)
                .FirstOrDefaultAsync(m => m.id == id);
            if (id != customer.id)
            {
                return NotFound();
            }

            if (customer.photo == null)
            {
                if (exCustomer.photo != null)
                {
                    customer.photo = exCustomer.photo;
                }
            }

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

                        if (!string.IsNullOrEmpty(customer.photo))
                        {
                            string fotoAnterior = Path.Combine(pathDestiny, customer.photo);
                            if (System.IO.File.Exists(fotoAnterior))
                                System.IO.File.Delete(fotoAnterior);
                        }

                        using (var filestream = new FileStream(Path.Combine(pathDestiny, fileDestiny), FileMode.Create))
                        {
                            filePhoto.CopyTo(filestream);
                            customer.photo = fileDestiny;
                        };
                    }
                }
                try
                {
                    customer.CustomerMedication.Clear();
                    CreateCustomerMedication(selectedMedication, customer);
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.id))
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
            ViewData["HealthcareSystemid"] = new SelectList(_context.healthcareSystem, "id", "name", customer.HealthcareSystemid);
            CreateCustomerMedication(selectedMedication, customer);
            MedicationDropDownList(customer);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.customer
                .Include(c => c.healthcareSystem)
                .FirstOrDefaultAsync(m => m.id == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var customer = await _context.customer.FindAsync(id);
            _context.customer.Remove(customer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
            return _context.customer.Any(e => e.id == id);
        }

        private void MedicationDropDownList(Customer customer = null)
        {
            var medicationQuery = from d in _context.medication
                                  orderby d.name
                                  select d;

            SelectList newSelectList = new SelectList(medicationQuery.AsNoTracking(), "id", "name");

            if (customer != null)
            {
                var customerMedication = _context.customerMedication.Where(sp => sp.Customerid == customer.id).Select(sp => sp.medication).ToList();

                foreach (var item in newSelectList)
                {
                    foreach (var medication in customerMedication)
                    {
                        if (medication.id.ToString() == item.Value)
                        {
                            item.Selected = true;
                        }
                    }
                }
            }

            ViewBag.Medications = newSelectList;
        }

        private void CreateCustomerMedication(string[] selectedMedication, Customer customer)
        {
            if (selectedMedication == null)
            {
                customer.CustomerMedication = new List<CustomerMedication>();
            }

            var selectedMedicationHS = new HashSet<string>(selectedMedication);

            foreach (var medication in _context.medication)
            {
                if (selectedMedicationHS.Contains(medication.id.ToString()))
                {
                    CustomerMedication newCustomerMedication = new CustomerMedication { customer = customer, medication = medication, Customerid = customer.id, Medicationid = medication.id };
                    customer.CustomerMedication.Add(newCustomerMedication);
                }
            }
        }
    }
}
