using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinFormu.Data;
using IzinFormu.Models;
using IzinFormu.Models.AccountViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IzinFormu.Controllers
{
    public class DepartmentController : Controller
    {
        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _ctx;

        public DepartmentController(ApplicationDbContext _ctx, UserManager<ApplicationUser> _usermanager)
        {
            this._usermanager = _usermanager;
            this._ctx = _ctx;
        }

        public IActionResult Index()
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var departman = _ctx.Departman.Where(s => s.DepartmanName == user.Department).Select(s => new DepartmanViewModel() { Id = s.Id, DepartmentName = s.DepartmanName.ToString(), ManagerId = s.Manager.ToString() }).FirstOrDefault();
            
            //user manager degilse gosterme
            //if (user.Email == departman.ManagerMail)
            //{
            //    return Content("Bu Departmana Bakamazsınız");
            //}
           // ViewBag.Id = departman.Id;
            return View();
        }
        public JsonResult GetsDepartment(int Id)
        {
            var departmentname = _ctx.Departman.Where(s => s.Id == Id).Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName }).ToList();           
            var alldepartmanuserlist = _ctx.Users.Where(s => s.Department == departmentname[0].DepartmentName).Select(s=>new RegisterViewModel() { Name=s.Name});
            return Json(alldepartmanuserlist);
        }

        public IActionResult AddUser()
        {
            ViewBag.users = _ctx.Users.ToList();
            return View();
        }
        [HttpPost]
        public IActionResult AddUser(RegisterViewModel model,int Id) {

            var editinguser = _ctx.Users.SingleOrDefault(a => a.Name==model.Name);
            if (editinguser != null)
            {
                editinguser.Department = _ctx.Departman.Where(s => s.Id == Id).Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName }).ToList()[0].DepartmentName;
                _ctx.SaveChanges();
            }
            return RedirectToAction("Index", "Department");
        }


    }
}