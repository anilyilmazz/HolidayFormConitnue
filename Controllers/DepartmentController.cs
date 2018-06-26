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
        public string StatusGenerator(int status)
        {
            if (status == 0)
            {
                return "Onay Bekliyor";
            }
            else if (status == 1)
            {
                return "Onaylandı";
            }
            else if (status == 2)
            {
                return "Yönetici Onaylamadı";
            }
            else
            {
                return "İznin durumu bozuk.";
            }

        }

        //DEPARTMENT SYSTEM

        [Authorize(Roles = "DepartmentManager")]
        public IActionResult Index()
        {

            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            
            var departman = _ctx.Departman.Where(s => s.DepartmanName == user.Department).Select(s => new DepartmanViewModel() { Id = s.Id, DepartmentName = s.DepartmanName.ToString(), ManagerId = s.Manager.ToString(),ManagerMail=s.Manager.Email }).FirstOrDefault();

            ViewBag.managermail = _ctx.Users.Where(s => s.Email == departman.ManagerMail).Select(s => new RegisterViewModel() { Name = s.Name }).FirstOrDefault().Name.ToString();
            ViewBag.Id = departman.Id;

            return View();
        }

        [Authorize(Roles = "DepartmentManager")]
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
            return Json(withoutmanager);
        }

        [Authorize(Roles = "DepartmentManager")]
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

        [Authorize(Roles = "DepartmentManager")]
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

        [Authorize(Roles = "DepartmentManager")]
        public IActionResult DeleteUser(string Email)
        {
            var editinguser = _ctx.Users.SingleOrDefault(a => a.Email == Email);
            editinguser.Department = "Departmanı Yok";
            editinguser.Manager = "Yöneticisi Yok";
            //MakeUser(editinguser);
            _ctx.SaveChanges();
            return RedirectToAction("Index", "Department");
        }

        //HOLIDAY STATUS SYSTEM
        [Authorize(Roles = "DepartmentManager")]
        public IActionResult ConfirmHoliday(int Id)
        {
            var editingholiday = _ctx.Holiday.Where(s => s.Id == Id).FirstOrDefault();
            editingholiday.Status = 1;
            _ctx.SaveChanges();
            return RedirectToAction("PendingHolidays", "Department");
        }
        [Authorize(Roles = "DepartmentManager")]
        public IActionResult DenyHoliday(int Id)
        {
            var editingholiday = _ctx.Holiday.Where(s => s.Id == Id).FirstOrDefault();
            editingholiday.Status = 2;
            _ctx.SaveChanges();
            return RedirectToAction("PendingHolidays", "Department");
        }

        [Authorize(Roles = "DepartmentManager")]
        public List<RegisterViewModel> WithoutManager(List<RegisterViewModel> withoutmanager, int Id)
        {
            foreach (var item in withoutmanager.ToList())
            {
                var managermail = _ctx.Departman.Where(s => s.Id == Id).Select(s => new DepartmanViewModel() { ManagerMail = s.Manager.Email }).FirstOrDefault().ManagerMail;
                if (item.Email == managermail)
                {
                    withoutmanager.Remove(item);
                }
            }

            return withoutmanager;
        }
        [Authorize(Roles = "DepartmentManager")]
        public IActionResult PendingHolidays()
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var departman = _ctx.Departman.Where(s => s.DepartmanName == user.Department).Select(s => new DepartmanViewModel() { Id = s.Id }).ToList();
            ViewBag.departman = departman[0].Id;
            return View();
        }
        [Authorize(Roles = "DepartmentManager")]
        public JsonResult GetsPendingHolidays(int holidayId)
        {
            var departmentname = _ctx.Departman.Where(s => s.Id == holidayId).Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName }).ToList();
            var usermail = _ctx.Users.Where(s => s.Department == departmentname[0].DepartmentName).Select(s => new RegisterViewModel() { Email = s.Email,Department=s.Department }).ToList();
            var userlist = WithoutManager(usermail, holidayId);
            List<int> holidaylist = new List<int>();
            var lsite = new List<HolidayViewModel>();
            var PendingHolidayList = new List<HolidayViewModel>();

            foreach (var item in userlist)
            {
                var holidayid = _ctx.Holiday.Where(s => s.User.Email == item.Email && s.Status == 0).Select(s => new HolidayViewModel() { Id = s.Id }).ToList();
                foreach(var item2 in holidayid)
                {
                    holidaylist.Add(item2.Id);
                }
                
            }
            foreach (var i in holidaylist)
            {
                var holiday = _ctx.Holiday.Where(s => s.Id == i).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id = s.Id, StartDateString = s.StartDate.ToString("dd-MM-yyyy"), EndDateString = s.EndDate.ToString("dd-MM-yyyy"), CreateDateString = s.CreateDate.ToString("dd-MM-yyyy"), HolidayTime = "5", Status = StatusGenerator(s.Status) }).ToList();
                holiday[0].HolidayTime = (holiday[0].EndDate - holiday[0].StartDate).ToString();
                PendingHolidayList.Add(holiday[0]);
             }

            return Json(PendingHolidayList);
        }

        [Authorize(Roles = "DepartmentManager")]
        public IActionResult ConfirmedHolidays()
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var departman = _ctx.Departman.Where(s => s.DepartmanName == user.Department).Select(s => new DepartmanViewModel() { Id = s.Id }).ToList();
            ViewBag.departman = departman[0].Id;
            return View();
        }
        [Authorize(Roles = "DepartmentManager")]
        public JsonResult GetsConfirmedHoliday(int holidayId)
        {
            var departmentname = _ctx.Departman.Where(s => s.Id == holidayId).Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName }).ToList();
            var usermail = _ctx.Users.Where(s => s.Department == departmentname[0].DepartmentName).Select(s => new RegisterViewModel() { Email = s.Email, Department = s.Department }).ToList();
            var userlist = WithoutManager(usermail, holidayId);
            List<int> holidaylist = new List<int>();
            var lsite = new List<HolidayViewModel>();
            var ConfirmedHolidayList = new List<HolidayViewModel>();

            foreach (var item in userlist)
            {
                var holidayid = _ctx.Holiday.Where(s => s.User.Email == item.Email && s.Status == 1).Select(s => new HolidayViewModel() { Id = s.Id }).ToList();
                foreach (var item2 in holidayid)
                {
                    holidaylist.Add(item2.Id);
                }

            }
            foreach (var i in holidaylist)
            {
                var holiday = _ctx.Holiday.Where(s => s.Id == i).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id = s.Id, StartDateString = s.StartDate.ToString("dd-MM-yyyy"), EndDateString = s.EndDate.ToString("dd-MM-yyyy"), CreateDateString = s.CreateDate.ToString("dd-MM-yyyy"), HolidayTime = "5", Status = StatusGenerator(s.Status) }).ToList();
                holiday[0].HolidayTime = (holiday[0].EndDate - holiday[0].StartDate).ToString();
                ConfirmedHolidayList.Add(holiday[0]);
            }

            return Json(ConfirmedHolidayList);
        }

        [Authorize(Roles = "DepartmentManager")]
        public IActionResult DeniedHolidays()
        {
            ApplicationUser user = _usermanager.FindByNameAsync(HttpContext.User.Identity.Name).Result;
            var departman = _ctx.Departman.Where(s => s.DepartmanName == user.Department).Select(s => new DepartmanViewModel() { Id = s.Id }).ToList();
            ViewBag.departman = departman[0].Id;
            return View();
        }
        [Authorize(Roles = "DepartmentManager")]
        public JsonResult GetsDeniedHolidays(int holidayId)
        {
            var departmentname = _ctx.Departman.Where(s => s.Id == holidayId).Select(s => new DepartmanViewModel() { DepartmentName = s.DepartmanName }).ToList();
            var usermail = _ctx.Users.Where(s => s.Department == departmentname[0].DepartmentName).Select(s => new RegisterViewModel() { Email = s.Email, Department = s.Department }).ToList();
            var userlist = WithoutManager(usermail, holidayId);
            List<int> holidaylist = new List<int>();
            var lsite = new List<HolidayViewModel>();
            var DeniedHolidayList = new List<HolidayViewModel>();

            foreach (var item in userlist)
            {
                var holidayid = _ctx.Holiday.Where(s => s.User.Email == item.Email && s.Status == 2).Select(s => new HolidayViewModel() { Id = s.Id }).ToList();
                foreach (var item2 in holidayid)
                {
                    holidaylist.Add(item2.Id);
                }

            }
            foreach (var i in holidaylist)
            {
                var holiday = _ctx.Holiday.Where(s => s.Id == i).Select(s => new HolidayViewModel() { CreateDate = s.CreateDate, Department = s.Department, EndDate = s.EndDate, Manager = s.Manager, RequestDate = s.RequestDate, StartDate = s.StartDate, User = s.User.Name, UserId = s.User.Id, Id = s.Id, StartDateString = s.StartDate.ToString("dd-MM-yyyy"), EndDateString = s.EndDate.ToString("dd-MM-yyyy"), CreateDateString = s.CreateDate.ToString("dd-MM-yyyy"), HolidayTime = "5", Status = StatusGenerator(s.Status) }).ToList();
                holiday[0].HolidayTime = (holiday[0].EndDate - holiday[0].StartDate).ToString();
                DeniedHolidayList.Add(holiday[0]);
            }

            return Json(DeniedHolidayList);
        }





    }
}