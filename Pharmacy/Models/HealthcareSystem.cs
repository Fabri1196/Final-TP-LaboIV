using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class HealthcareSystem
    {
        public int id { get; set; }

        [Display(Name = "Nombre")]
        public string name { get; set; }
    }
}
