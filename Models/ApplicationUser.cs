using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IzinFormu.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {

        public string Name { get; set; }
        public string Department { get; set; }
        public string Manager { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
