using Microsoft.AspNetCore.Mvc.Rendering;
using Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.ModelsView
{
    public class MedicationViewModel
    {
        public List<Medication> MedicationList { get; set; }
        public SelectList CategoryList { get; set; }
        public SelectList LabList { get; set; }
        public string findName { get; set; }
        public pager pager { get; set; }
    }
}
