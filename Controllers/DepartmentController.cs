using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinFormu.Data;
using IzinFormu.Models;
using IzinFormu.Models.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace IzinFormu.Controllers
{
    public class DepartmentController : Controller
    {
        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _ctx;
        private RoleManager<IdentityRole> _rolemanager;

        public DepartmentController(ApplicationDbContext _ctx, UserManager<ApplicationUser> _usermanager)
        {
            this._usermanager = _usermanager;
            this._ctx = _ctx;
            this._rolemanager = _rolemanager;
        }

        public void MakeUser(ApplicationUser user)
        {
            IdentityRole role = _rolemanager.FindByNameAsync("User").Result;
            if (role == null)
            {
                var result = _rolemanager.CreateAsync(new IdentityRole("User")).Result;
                role = _rolemanager.FindByNameAsync("User").Result;
            }
            var userroleresult = _usermanager.AddToRoleAsync(user, "User").Result;
            var userroleresult2 = _usermanager.RemoveFromRoleAsync(user, "DepartmentManager").Result;
            var userroleresult3 = _usermanager.RemoveFromRoleAsync(user, "Admin").Result;
        }

        [Authorize(Roles = "DepartmentManager")]
        public IActionResult Index()
        {

            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            
            var departman = _ctx.Departman.Where(s => s.DepartmanName == user.Department).Select(s => new DepartmanViewModel() { Id = s.Id, DepartmentName = s.DepartmanName.ToString(), ManagerId = s.Manager.ToString(),ManagerMail=s.Manager.Email }).FirstOrDefault();

            ViewBag.managermail = _ctx.Users.Where(s => s.Email == departman.ManagerMail).Select(s => new RegisterViewModel() { Name = s.Name }).FirstOrDefault().Name.ToString();
            ViewBag.Id = departman.Id;

            return View();
        }
        public JsonResult GetsDepartment(int Id)
        {
            var departmentname = _ctx.Departman.Where(s => s.Id == Id).Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName }).ToList();
            var alldepartmanuserlist = _ctx.Users.Where(s => s.Department == departmentname[0].DepartmentName).Select(s => new RegisterViewModel() { Name = s.Name, Email = s.Email }).ToList();
            var withoutmanager = alldepartmanuserlist;
            foreach (var item in alldepartmanuserlist.ToList())
            {
               var managermail = _ctx.Departman.Where(s => s.Id == Id).Select(s => new DepartmanViewModel() { ManagerMail = s.Manager.Email }).FirstOrDefault().ManagerMail;
                if (item.Email == managermail)
                {
                    withoutmanager.Remove(item);
                }
            }
            return Json(alldepartmanuserlist);
        }
        public IActionResult AddUser()
        {
            var withoutmanagers = _ctx.Users.ToList();
            foreach ( var item in withoutmanagers.ToList())
            {
                if((item.Manager == item.Name) || (!(item.Department == "Departmanı Yok")))
                {
                    withoutmanagers.Remove(item);
                }
            }
            ViewBag.users = withoutmanagers;
            return View();
        }
        [HttpPost]
        public IActionResult AddUser(RegisterViewModel model,int Id) {

            var editinguser = _ctx.Users.SingleOrDefault(a => a.Name==model.Name);
            if (editinguser != null)
            {
                editinguser.Department = _ctx.Departman.Where(s => s.Id == Id).Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName }).ToList()[0].DepartmentName;
                var gecici = _ctx.Departman.Where(s => s.Id == Id).Select(s => new DepartmanViewModel() { ManagerMail=s.Manager.ToString()}).ToList()[0].ManagerMail.ToString();
                editinguser.Manager = _ctx.Users.Where(s => s.Email == gecici).Select(s => new RegisterViewModel() { Name = s.Name }).FirstOrDefault().Name.ToString();
                _ctx.SaveChanges();
            }
            return RedirectToAction("Index", "Department");
        }
        public IActionResult DeleteUser(string Email)
        {
            var editinguser = _ctx.Users.SingleOrDefault(a => a.Email == Email);
            editinguser.Department = "Departmanı Yok";
            editinguser.Manager = "Yöneticisi Yok";
            //MakeUser(editinguser);
            _ctx.SaveChanges();
            return RedirectToAction("Index", "Department");
        }



    }
}