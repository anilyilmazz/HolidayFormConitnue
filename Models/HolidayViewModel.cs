using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IzinFormu.Models
{
    public class HolidayViewModel
    {
        [Required]
        [Display(Name = "İşe Giriş Tarihi")]
        public DateTime CreateDate { get; set; }

        [Required]
        [Display(Name = "İzin Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Required]
        [Display(Name = "İzin Bitiş Tarihi")]
        public DateTime EndDate { get; set; }

        [Required]
        [Display(Name = "Form Tarihi")]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Departman")]
        public string Department { get; set; }

        [Display(Name = "Yonetici")]
        public string Manager { get; set; }

        [Display(Name = "Kullanıcı")]
        public string User { get; set; }

        [Display(Name = "UserId")]
        public string UserId { get; set; }

        [Display(Name = "Holiday Id")]
        public int Id { get; set; }

        public string StartDateString { get; set; }
        public string EndDateString { get; set; }
        public string CreateDateString { get; set; }
        public string HolidayTime { get; set; }


    }
}
