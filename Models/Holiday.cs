using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IzinFormu.Models
{
    public class Holiday
    {
        public int Id { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime RequestDate { get; set; }
        public string Department { get; set; }
        public string Manager { get; set; }
        
    }
}
