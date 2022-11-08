using Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.ModelsView
{
    public class CategoryViewModel
    {
        public List<Category> CategoryList { get; set; }
        public pager pager { get; set; }
    }
}
