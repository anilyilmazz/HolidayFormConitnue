using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using IzinFormu.Models;
using Microsoft.AspNetCore.Identity;
using IzinFormu.Data;
using Microsoft.AspNetCore.Authorization;

namespace IzinFormu.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _ctx;
        private RoleManager<IdentityRole> _rolemanager;

        public HomeController(UserManager<ApplicationUser> _usermanager, RoleManager<IdentityRole> _rolemanager ,ApplicationDbContext _ctx)
        {
            this._usermanager = _usermanager;
            this._rolemanager = _rolemanager;
            this._ctx = _ctx;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public IActionResult AssignRole()
        {

            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            IdentityRole role = _rolemanager.FindByNameAsync("Admin").Result;
            if (role == null)
            {
                var result = _rolemanager.CreateAsync(new IdentityRole("Admin")).Result;

                if (!result.Succeeded)
                {
                    return Content("Unsuccesful");
                }

                role = _rolemanager.FindByNameAsync("Admin").Result;

            }

            var userroleresult = _usermanager.AddToRoleAsync(user, "Admin").Result;

            if (!userroleresult.Succeeded)
            {
                return Content("userroleresult Unsuccesful");
            }
            

            return Content("OK");

        }

        public IActionResult HolidayCreate()
        {
            return View();
        }
        public IActionResult HolidayIndex()
        {
            return View();
        }
    }
}
