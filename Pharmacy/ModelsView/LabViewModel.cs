using Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.ModelsView
{
    public class LabViewModel
    {
        public List<Lab> LabList { get; set; }
        public pager pager { get; set; }
    }
}
