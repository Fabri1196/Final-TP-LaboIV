using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class Customer
    {
        public Customer()
        {
            this.CustomerMedication = new HashSet<CustomerMedication>();
        }

        public int id { get; set; }

        [Display(Name = "Apellido")]
        public string surname { get; set; }

        [Display(Name = "Nombre")]
        public string name { get; set; }

        [Display(Name = "Foto")]
        public string photo { get; set; }

        [Display(Name = "Dirección")]
        public string address { get; set; }

        public int HealthcareSystemid { get; set; }
        [Display(Name = "Obra Social")]
        public HealthcareSystem healthcareSystem { get; set; }

        [Display(Name = "Medicación")]
        public ICollection<CustomerMedication> CustomerMedication { get; set; }
    }
}
