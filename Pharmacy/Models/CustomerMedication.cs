using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pharmacy.Models
{
    public class CustomerMedication
    {
        public int id { get; set; }

        public int Customerid { get; set; }
        public Customer customer { get; set; }

        public int Medicationid { get; set; }
        public Medication medication { get; set; }
    }
}
