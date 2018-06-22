using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace IzinFormu.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        //[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Şifre")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Tekrar Şifre")]
        [Compare("Password", ErrorMessage = "Girilen Şifreler Uyuşmadı.")]
        public string ConfirmPassword { get; set; }

        
        [Display(Name = "İsim Soyisim")]
        public string Name { get; set; }
        
        [Display(Name = "Departman")]
        public string Department { get; set; }

        
        [Display(Name = "Yönetici")]
        public string Manager { get; set; }

        
        [Display(Name = "İşe Giriş Tarihi")]
        public DateTime CreateDate { get; set; }



    }
}
