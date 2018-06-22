using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IzinFormu.Models
{
    public class DepartmanViewModel
    {
        [Required]
        [Display(Name = "Departman Adı")]
        public string DepartmentName { get; set; }

        [Display(Name = "Departman Yöneticisi")]
        public string ManagerId { get; set; }

        [Display(Name = "Departman Id")]
        public int Id { get; set; }

        [Display(Name ="Yönetici Mail Adresi")]
        public string ManagerMail { get; set; }
    }
}
