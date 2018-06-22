using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinFormu.Data;
using IzinFormu.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IzinFormu.Controllers
{
    public class DepartmentController : Controller
    {
        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _ctx;
        private RoleManager<IdentityRole> _rolemanager;


        public IActionResult Index()
        {
            return View();
        }
        public JsonResult GetsDepartment()
        {
            var alldepartmanlist = _ctx.Departman.Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName, ManagerMail = s.Manager.Email, Id = s.Id });
            return Json(alldepartmanlist);
        }



    }
}