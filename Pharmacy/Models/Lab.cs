using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class Lab
    {
        public int id { get; set; }

        [Display(Name = "Nombre")]
        public string name { get; set; }

        [Display(Name = "Email")]
        public string email { get; set; }

        [Display(Name = "Ciudad")]
        public string city { get; set; }

        [Display(Name = "País")]
        public string country { get; set; }
    }
}
