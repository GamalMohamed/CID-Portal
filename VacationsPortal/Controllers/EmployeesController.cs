using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using VacationsPortal.Models;

namespace VacationsPortal.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        private string GetCurrentUserEmail()
        {
            return ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
        }

        // GET: Employees
        public ActionResult Index()
        {
            return View(_db.EmployeesViews.ToList());
        }

        // IMP NOTE: THIS ROUTE IS FOR DEVELOPMENT PURPOSES ONLY!!
        //public ActionResult FillEmployeesView()
        //{
        //    var emps = _db.Employees.ToList();
        //    string directlineName = "", dottedlineName = "";
        //    foreach (var emp in emps)
        //    {
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

        //        var empvw = new EmployeesView();
        //        empvw.Id = emp.Id;
        //        empvw.BasedOut = emp.contact.BasedOut;
        //        empvw.Email = emp.contact.Email;
        //        empvw.HiringDate = emp.hiringDate;
        //        empvw.Name = emp.contact.FirstName + ' ' + emp.contact.LastName;
        //        empvw.PhoneNumber = emp.contact.PhoneNumber;
        //        empvw.VacationBalance = emp.VacationBalance;
        //        empvw.VacationsCarryOver = emp.VacationsCarryOver;
        //        empvw.PassportNumber = emp.contact.PassportNumber;
        //        empvw.Role = role;
        //        empvw.Workload = workload;
        //        empvw.DirectLine = directlineName;
        //        empvw.DottedLine = dottedlineName;

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
            ViewBag.Id = new SelectList(_db.contacts, "Id", "UserName");
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId");
            ViewBag.dottedLine = new SelectList(_db.Employees, "Id", "employeeId");
            ViewBag.RoleID = new SelectList(_db.Roles, "Id", "roleName");
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId");
            ViewBag.Workload = new SelectList(_db.Workloads, "Id", "WorkloadName");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _db.Employees.Add(employee);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id = new SelectList(_db.contacts, "Id", "UserName", employee.Id);
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId", employee.directLine);
            ViewBag.dottedLine = new SelectList(_db.Employees, "Id", "employeeId", employee.dottedLine);
            ViewBag.RoleID = new SelectList(_db.Roles, "Id", "roleName", employee.RoleID);
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId", employee.directLine);
            ViewBag.Workload = new SelectList(_db.Workloads, "Id", "WorkloadName", employee.Workload);
            return View(employee);
        }

        // GET: Employees/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Employee employee = _db.Employees.Find(id);
            if (employee == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id = new SelectList(_db.contacts, "Id", "UserName", employee.Id);
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId", employee.directLine);
            ViewBag.dottedLine = new SelectList(_db.Employees, "Id", "employeeId", employee.dottedLine);
            ViewBag.RoleID = new SelectList(_db.Roles, "Id", "roleName", employee.RoleID);
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId", employee.directLine);
            ViewBag.Workload = new SelectList(_db.Workloads, "Id", "WorkloadName", employee.Workload);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Employee employee)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(employee).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id = new SelectList(_db.contacts, "Id", "UserName", employee.Id);
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId", employee.directLine);
            ViewBag.dottedLine = new SelectList(_db.Employees, "Id", "employeeId", employee.dottedLine);
            ViewBag.RoleID = new SelectList(_db.Roles, "Id", "roleName", employee.RoleID);
            ViewBag.directLine = new SelectList(_db.Employees, "Id", "employeeId", employee.directLine);
            ViewBag.Workload = new SelectList(_db.Workloads, "Id", "WorkloadName", employee.Workload);
            return View(employee);
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
