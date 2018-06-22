using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IzinFormu.Models
{
    public class DepartmentsViewModel
    {
        [Required]
        [Display(Name = "Departman Adı")]
        public string DDepartmentName { get; set; }

        [Display(Name ="Departman Yöneticisi")]
        public string User { get; set; }

        [Display(Name = "Departman Id")]
        public int Id { get; set; }
    }
}
