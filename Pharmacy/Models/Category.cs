using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class Category
    {
        public int id { get; set; }

        [Display(Name = "Categoria")]
        public string description { get; set; }
    }
}
