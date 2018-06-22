using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IzinFormu.Models
{
    public class Departman
    {
        public int Id { get; set; }
        public string DepartmanName { get; set; }
        public ApplicationUser Manager { get; set; }

     }
}
