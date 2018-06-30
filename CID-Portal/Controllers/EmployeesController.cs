using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Policy;
using System.Web;
using System.Web.Mvc;
using VacationsPortal.Models;
using VacationsPortal.ViewModels;

namespace VacationsPortal.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        private bool IsUserAuthenticated()
        {
            var loggedUserEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            var authUser = _db.AuthUsers.FirstOrDefault(u => u.Email == loggedUserEmail);
            return authUser != null;
            //return true;
        }

        public ActionResult MarkResigned(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var employee = _db.Employees.FirstOrDefault(e => e.Id == id);
            if (employee != null)
            {
                employee.Resigned = true;
            }
            _db.SaveChanges();

            var empview = _db.EmployeesViews.FirstOrDefault(e => e.Id == id);
            if (empview != null)
            {
                empview.Resigned = true;
            }
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult ClearCarryOver()
        {
            _db.Database.ExecuteSqlCommand("UPDATE dbo.EmployeesView SET VacationsCarryOver = 0");
            _db.Database.ExecuteSqlCommand("UPDATE dbo.Employees SET VacationsCarryOver = 0");
            return RedirectToAction("Index");
        }

        // GET: Employees
        public ActionResult Index()
        {
            if (IsUserAuthenticated())
            {
                return View(_db.EmployeesViews.Where(e => e.Resigned.Value == false).ToList());
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        public ActionResult Resigned()
        {
            if (IsUserAuthenticated())
            {
                return View("Index",_db.EmployeesViews.Where(e => e.Resigned.Value).ToList());
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        public ActionResult NullResigned()
        {
            if (IsUserAuthenticated())
            {
                return View("Index", _db.EmployeesViews.Where(e => e.Resigned == null).ToList());
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        // IMP NOTE: THIS ROUTE IS FOR DEVELOPMENT PURPOSES ONLY!!
        //public ActionResult FillEmployeesView()
        //{
        //    var emps = _db.Employees.ToList();
        //    foreach (var emp in emps)
        //    {
        //        string directlineName = "", dottedlineName = "";
        //        if (emp.directLine != null)
        //        {
        //            var dirline = emps.FirstOrDefault(e => e.Id == emp.directLine);
        //            if (dirline != null)
        //            {
        //                directlineName = dirline.contact.FullName;
        //            }
        //        }

        //        if (emp.dottedLine != null)
        //        {
        //            var dotline = emps.FirstOrDefault(e => e.Id == emp.dottedLine);
        //            if (dotline != null)
        //            {
        //                dottedlineName = dotline.contact.FullName;
        //            }
        //        }

        //        string role = "", workload = "";
        //        if (emp.Role != null)
        //        {
        //            role = emp.Role.roleName;
        //        }
        //        if (emp.Workload != null)
        //        {
        //            workload = emp.Workload1.WorkloadName;
        //        }

        //        var empvw = new EmployeesView
        //        {
        //            Id = emp.Id,
        //            BasedOut = emp.contact.BasedOut,
        //            Email = emp.contact.Email,
        //            HiringDate = emp.hiringDate,
        //            Name = emp.contact.FirstName + ' ' + emp.contact.LastName,
        //            PhoneNumber = emp.contact.PhoneNumber,
        //            VacationBalance = emp.VacationBalance,
        //            VacationsCarryOver = emp.VacationsCarryOver,
        //            Role = role,
        //            Workload = workload,
        //            DirectLine = directlineName,
        //            DottedLine = dottedlineName,
        //            Resigned = emp.Resigned
        //        };

        //        _db.EmployeesViews.Add(empvw);

        //        _db.SaveChanges();
        //    }

        //    return RedirectToAction("Index");
        //}

        // GET: Employees/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = _db.EmployeesViews.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            return View(employee);
        }

        // GET: Employees/Create
        public ActionResult Create()
        {
            if (IsUserAuthenticated())
            {
                ViewBag.EmployeesList = new SelectList(_db.contacts.ToList(), "Id", "FullName");
                ViewBag.RolesList = new SelectList(_db.Roles.ToList(), "Id", "roleName");
                ViewBag.WorkloadsList = new SelectList(_db.Workloads.ToList(), "Id", "WorkloadName");
                ViewBag.CountriesList = new SelectList(_db.Countries.ToList(), "CountryName", "CountryName");
                ViewBag.CitiesList = new SelectList(_db.Cities.ToList(), "Id", "Name");
                return View();
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeViewModel employeevm)
        {
            var emps = _db.Employees.ToList();
            if (ModelState.IsValid)
            {
                var nametokens = employeevm.Name.Split(' ');
                var firstname = nametokens[0];
                var lastname = "";
                if (nametokens.Length > 1)
                {
                    lastname = employeevm.Name.Substring(firstname.Length + 1, employeevm.Name.Length - firstname.Length - 1);
                }
                var city = _db.Cities.FirstOrDefault(c => c.Id == employeevm.BaseCity.Id);
                // Add contact
                var contact = new contact()
                {
                    FullName = employeevm.Name,
                    FirstName = firstname,
                    LastName = lastname,
                    BasedOut = employeevm.BasedOut.CountryName,
                    UserName = employeevm.UserName,
                    Email = employeevm.UserName + "@microsoft.com",
                    PhoneNumber = employeevm.PhoneNumber,
                    BaseCity = employeevm.BaseCity.Id,
                    City = city
                };

                _db.contacts.Add(contact);
                _db.SaveChanges();

                var contactDb = _db.contacts.FirstOrDefault(e => e.UserName == employeevm.UserName);
                var roole = _db.Roles.FirstOrDefault(r => r.Id == employeevm.Role.Id);
                var woorkload = _db.Workloads.FirstOrDefault(w => w.Id == employeevm.Workload.Id);
                if (contactDb != null)
                {
                    // Add Employee
                    contactDb.Employee = new Employee()
                    {
                        Id = contactDb.Id,
                        VacationsCarryOver = employeevm.VacationsCarryOver ?? 0,
                        VacationBalance = employeevm.VacationBalance ?? 0,
                        hiringDate = employeevm.HiringDate,
                        RoleID = employeevm.Role.Id,
                        Workload = employeevm.Workload.Id,
                        Role = roole,
                        Workload1 = woorkload,
                        RoleTypeID = 0,
                        CIDW8RoleTypes = _db.CIDW8RoleTypes.FirstOrDefault(r => r.Id == 0),
                        directLine = employeevm.DirectLine.Id,
                        dottedLine = employeevm.DottedLine.Id,
                        CasualLeaveBalance = 0,
                        Resigned = false,
                        Provisioned = true,
                        Active = true,
                        StatusUpdates = "Welcome me to CID on Windows"
                    };
                    _db.SaveChanges();

                    // Add also to EmployeesView
                    string directlineName = "", dottedlineName = "";
                    if (contactDb.Employee.directLine != null)
                    {
                        var dirline = emps.FirstOrDefault(e => e.Id == contactDb.Employee.directLine);
                        if (dirline != null)
                        {
                            directlineName = dirline.contact.FullName;
                        }
                    }

                    if (contactDb.Employee.dottedLine != null)
                    {
                        var dotline = emps.FirstOrDefault(e => e.Id == contactDb.Employee.dottedLine);
                        if (dotline != null)
                        {
                            dottedlineName = dotline.contact.FullName;
                        }
                    }

                    string role = "", workload = "";
                    if (contactDb.Employee.Role != null)
                    {
                        role = contactDb.Employee.Role.roleName;
                    }
                    if (contactDb.Employee.Workload1 != null)
                    {
                        workload = contactDb.Employee.Workload1.WorkloadName;
                    }
                    var empview = new EmployeesView()
                    {
                        Id = contactDb.Id,
                        BasedOut = contactDb.BasedOut,
                        Email = contactDb.Email,
                        HiringDate = contactDb.Employee.hiringDate,
                        Name = contactDb.FullName,
                        VacationBalance = contactDb.Employee.VacationBalance,
                        VacationsCarryOver = contactDb.Employee.VacationsCarryOver,
                        PhoneNumber = contactDb.PhoneNumber,
                        Role = role,
                        Workload = workload,
                        DirectLine = directlineName,
                        DottedLine = dottedlineName,
                        Resigned = false
                    };
                    _db.EmployeesViews.Add(empview);
                    _db.SaveChanges();
                }

                return RedirectToAction("Index");
            }

            return View("Error");
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var contact = _db.contacts.Find(id);
            if (contact == null)
            {
                return HttpNotFound();
            }

            var dirline = _db.Employees.FirstOrDefault(e => e.Id == contact.Employee.directLine.Value);
            var dotline = _db.Employees.FirstOrDefault(e => e.Id == contact.Employee.dottedLine.Value);
            var basedOut = _db.Countries.FirstOrDefault(e => e.CountryName == contact.BasedOut);
            var resigned = contact.Employee.Resigned != null && contact.Employee.Resigned.Value;
            var employeevm = new EmployeeViewModel()
            {
                Id = contact.Id,
                Workload = contact.Employee.Workload1,
                Role = contact.Employee.Role,
                UserName = contact.UserName,
                HiringDate = contact.Employee.hiringDate,
                Name = contact.FullName,
                PhoneNumber = contact.PhoneNumber,
                VacationBalance = contact.Employee.VacationBalance,
                VacationsCarryOver = contact.Employee.VacationsCarryOver,
                DirectLine = dirline,
                DottedLine = dotline,
                BasedOut = basedOut,
                Resigned = resigned,
                BaseCity = contact.City
            };

            ViewBag.EmployeesList = new SelectList(_db.contacts.ToList(), "Id", "FullName");
            ViewBag.RolesList = new SelectList(_db.Roles.ToList(), "Id", "roleName");
            ViewBag.WorkloadsList = new SelectList(_db.Workloads.ToList(), "Id", "WorkloadName");
            ViewBag.CountriesList = new SelectList(_db.Countries.ToList(), "CountryName", "CountryName");
            ViewBag.CitiesList = new SelectList(_db.Cities.ToList(), "Id", "Name");
            return View(employeevm);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeViewModel employeevm)
        {
            if (ModelState.IsValid)
            {
                // Update contact and employee
                var contact = _db.contacts.FirstOrDefault(e => e.Id == employeevm.Id);
                if (contact != null)
                {
                    contact.FullName = employeevm.Name;
                    var nametokens = employeevm.Name.Split(' ');
                    var firstname = nametokens[0];
                    var lastname = "";
                    if (nametokens.Length > 1)
                    {
                        lastname = employeevm.Name.Substring(firstname.Length + 1, employeevm.Name.Length - firstname.Length - 1);
                    }
                    contact.FirstName = firstname;
                    contact.LastName = lastname;
                    contact.BasedOut = employeevm.BasedOut.CountryName;
                    contact.City = employeevm.BaseCity;
                    contact.UserName = employeevm.UserName;
                    contact.Email = employeevm.UserName + "@microsoft.com";
                    contact.PhoneNumber = employeevm.PhoneNumber;
                    contact.Employee.VacationsCarryOver = employeevm.VacationsCarryOver ?? 0;
                    contact.Employee.VacationBalance = employeevm.VacationBalance ?? 0;
                    contact.Employee.hiringDate = employeevm.HiringDate;
                    contact.Employee.directLine = employeevm.DirectLine.Id;
                    contact.Employee.dottedLine = employeevm.DottedLine.Id;

                    var roole = _db.Roles.FirstOrDefault(r => r.Id == employeevm.Role.Id);
                    var woorkload = _db.Workloads.FirstOrDefault(w => w.Id == employeevm.Workload.Id);
                    var city = _db.Cities.FirstOrDefault(c => c.Id == employeevm.BaseCity.Id);
                    contact.Employee.RoleID = employeevm.Role.Id;
                    contact.Employee.Workload = employeevm.Workload.Id;
                    contact.Employee.Role = roole;
                    contact.Employee.Workload1 = woorkload;
                    contact.Employee.Resigned = employeevm.Resigned;
                    contact.BaseCity = employeevm.BaseCity.Id;
                    contact.City = city;
                    
                    _db.SaveChanges();

                    // Update EmployeesView
                    var empview = _db.EmployeesViews.FirstOrDefault(e => e.Id == contact.Id);
                    var emps = _db.Employees.ToList();
                    string directlineName = "", dottedlineName = "";
                    if (contact.Employee.directLine != null)
                    {
                        var dirline = emps.FirstOrDefault(e => e.Id == contact.Employee.directLine);
                        if (dirline != null)
                        {
                            directlineName = dirline.contact.FullName;
                        }
                    }

                    if (contact.Employee.dottedLine != null)
                    {
                        var dotline = emps.FirstOrDefault(e => e.Id == contact.Employee.dottedLine);
                        if (dotline != null)
                        {
                            dottedlineName = dotline.contact.FullName;
                        }
                    }

                    string role = "", workload = "";
                    if (contact.Employee.Role != null)
                    {
                        role = contact.Employee.Role.roleName;
                    }
                    if (contact.Employee.Workload1 != null)
                    {
                        workload = contact.Employee.Workload1.WorkloadName;
                    }
                    if (empview != null)
                    {
                        empview.Id = contact.Id;
                        empview.BasedOut = contact.BasedOut;
                        empview.Email = contact.Email;
                        empview.HiringDate = contact.Employee.hiringDate;
                        empview.Name = contact.FullName;
                        empview.VacationBalance = contact.Employee.VacationBalance;
                        empview.VacationsCarryOver = contact.Employee.VacationsCarryOver;
                        empview.PhoneNumber = contact.PhoneNumber;
                        empview.Role = role;
                        empview.Workload = workload;
                        empview.DirectLine = directlineName;
                        empview.DottedLine = dottedlineName;
                        empview.Resigned = contact.Employee.Resigned;
                    }
                    _db.SaveChanges();
                }
                return RedirectToAction("Index");
            }

            return View(employeevm);
        }

        // GET: Employees/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var employee = _db.Employees.Find(id);
            var contact = _db.contacts.Find(id);
            var employeeview = _db.EmployeesViews.Find(id);
            if (employee == null || contact == null || employeeview == null)
            {
                return HttpNotFound();
            }
            _db.Employees.Remove(employee);
            _db.SaveChanges();

            _db.contacts.Remove(contact);
            _db.SaveChanges();

            _db.EmployeesViews.Remove(employeeview);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
