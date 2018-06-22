using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IzinFormu.Data;
using IzinFormu.Models;
using IzinFormu.Models.AccountViewModels;
using IzinFormu.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace IzinFormu.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {

        private UserManager<ApplicationUser> _usermanager;
        private ApplicationDbContext _ctx;
        private RoleManager<IdentityRole> _rolemanager;

        public AdminController(UserManager<ApplicationUser> _usermanager, RoleManager<IdentityRole> _rolemanager, ApplicationDbContext _ctx)
        {
            this._usermanager = _usermanager;
            this._rolemanager = _rolemanager;
            this._ctx = _ctx;
        }
        public IActionResult Index()
        {

            var allusers = _ctx.Users;
            return View();
        }

        //USER METHODS

        public IActionResult UserRegister(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            ViewBag.departments = _ctx.Departman.ToList();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UserRegister(RegisterViewModel model,string returnurl = null)
        {
            ViewData["ReturnUrl"] = returnurl;
            var user = new ApplicationUser { UserName = model.Email, Email = model.Email,Name = model.Name,Department=model.Department ,Manager=model.Manager,CreateDate=model.CreateDate};
            var gecici = _ctx.Departman.Where(s => s.DepartmanName == model.Department).Select(s=> new DepartmanViewModel() { Id=s.Id, DepartmentName =s.DepartmanName.ToString(),ManagerId = s.Manager.ToString()}).FirstOrDefault();
            var managername = _ctx.Users.Where(s => s.Email == gecici.ManagerId).FirstOrDefault();
            user.Manager = managername.Name.ToString();
            var result = await _usermanager.CreateAsync(user, model.Password);
            return RedirectToAction("UserIndex", "Admin");
        }
        public IActionResult UserIndex()
        {      
            return View(_ctx.Users.ToList());
        }

        //ADMIN HOLIDAY METHODS

        public IActionResult HolidayIndex()
        {
            return View();
        }
        public JsonResult GetsHoliday()
        {
            var allholidaylist = _ctx.Holiday.Select(s => new HolidayViewModel() {CreateDate = s.CreateDate, Department = s.User.Department, EndDate = s.EndDate, Manager = s.User.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id=s.Id , StartDateString = s.StartDate.ToString("dd-MM-yyyy"), EndDateString = s.EndDate.ToString("dd-MM-yyyy"), CreateDateString = s.CreateDate.ToString("dd-MM-yyyy") }).ToList();

            foreach (var i in allholidaylist)
            {
                int holidaycount = -1;
                int fridaycount = 0;
                for (DateTime friday = i.StartDate; friday <= i.EndDate; friday = friday.AddDays(1))
                {
                   string deneme =  friday.Year.ToString();
                    holidaycount++;
                    if(friday.DayOfWeek == DayOfWeek.Friday)
                    {
                        fridaycount++;
                    }
                    
                }
                i.HolidayTime = (holidaycount+fridaycount).ToString();

            }

            return Json(allholidaylist);
        }

        //DEPARTMENT METHODS

        [Authorize]
        public IActionResult DepartmentCreate()
        {
            ViewBag.users = _ctx.Users.ToList();
            return View();
        }
        [Authorize]
        public void MakeManager(ApplicationUser user)
        {
            IdentityRole role = _rolemanager.FindByNameAsync("DepartmentManager").Result;
            if (role == null)
            {
                var result = _rolemanager.CreateAsync(new IdentityRole("DepartmentManager")).Result;
                role = _rolemanager.FindByNameAsync("DepartmentManager").Result;
            }
            var userroleresult = _usermanager.AddToRoleAsync(user, "DepartmentManager").Result;

        }

        [Authorize]
        [HttpPost]
        public IActionResult DepartmentCreate(DepartmanViewModel model)
        {
            var Departman = new Departman(); ;
            Departman.DepartmanName = model.DepartmentName;
            Departman.Manager = _ctx.Users.Where(a => a.Email == model.ManagerMail).FirstOrDefault();
            MakeManager(Departman.Manager);
            _ctx.Departman.Add(Departman);
            _ctx.SaveChanges();
            return RedirectToAction("DepartmentIndex", "Admin");
        }
        public IActionResult DepartmentIndex()
        {
            return View();
        }
        public JsonResult GetsDepartment()
        {
            var alldepartmanlist = _ctx.Departman.Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName, ManagerMail = s.Manager.Email,Id=s.Id });
            return Json(alldepartmanlist);
        }
        [Authorize]
        public IActionResult DepartmentDelete(int Id)
        {
            var deletingdepartman = new Departman { Id=Id };
            _ctx.Departman.Attach(deletingdepartman);
            _ctx.Departman.Remove(deletingdepartman);
            _ctx.SaveChanges();
            return RedirectToAction("DepartmentIndex", "Admin");
        }

        [Authorize]
        public IActionResult DepartmentEdit(int Id)
        {
            ViewBag.users = _ctx.Users.ToList();
            ViewBag.departmanname = _ctx.Departman.Where(a => a.Id == Id).Select(s => s.DepartmanName).FirstOrDefault();
            ViewBag.managermail = _ctx.Departman.Where(a => a.Id == Id).Select(s => s.Manager.Email).FirstOrDefault();

            return View();
        }
        [HttpPost]
        [Authorize]
        public IActionResult DepartmentEdit(DepartmanViewModel model, int Id)
        {
            
            var editingdepartment = _ctx.Departman.SingleOrDefault(a => a.Id == Id);
            if (editingdepartment != null)
            {
                editingdepartment.DepartmanName = model.DepartmentName;
                editingdepartment.Manager = _ctx.Users.Where(a => a.Email == model.ManagerMail).FirstOrDefault();
                MakeManager(editingdepartment.Manager);
                _ctx.SaveChanges();
            }
           
            return RedirectToAction("DepartmentIndex", "Admin");
        }
    }
}