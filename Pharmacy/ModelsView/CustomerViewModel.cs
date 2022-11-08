using Microsoft.AspNetCore.Mvc.Rendering;
using Pharmacy.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.ModelsView
{
    public class CustomerViewModel
    {
        public List<Customer> CustomerList { get; set; }
        public SelectList HealthcareSystemList { get; set; }
        public string findSurname { get; set; }
        public string findName { get; set; }
        public pager pager { get; set; }
    }

    public class pager
    {
        public int rows { get; set; }
        public int rowXpag { get; set; }
        public int currentPage { get; set; }
        public int totalPag => (int)Math.Ceiling((decimal)rows / rowXpag);
        public Dictionary<string, string> ValuesQueryString { get; set; } = new Dictionary<string, string>();
    }
}
