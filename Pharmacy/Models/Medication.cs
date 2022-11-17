using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class Medication
    {
        public Medication()
        {
            this.CustomerMedication = new HashSet<CustomerMedication>();
        }

        public int id { get; set; }

        [Display(Name = "Nombre")]
        public string name { get; set; }

        [Display(Name = "Precio")]
        public int price { get; set; }

        public int Categoryid { get; set; }
        [Display(Name = "Categoria")]
        public Category category { get; set; }

        public int Labid { get; set; }
        [Display(Name = "Laboratorio")]
        public Lab lab { get; set; }

        [Display(Name = "Foto")]
        public string photo { get; set; }

        [Display(Name = "Cliente")]
        public ICollection<CustomerMedication> CustomerMedication { get; set; }
    }
}
