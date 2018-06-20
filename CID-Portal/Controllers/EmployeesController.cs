﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using VacationsPortal.Models;
using VacationsPortal.ViewModels;

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
        public ActionResult FillEmployeesView()
        {
            var emps = _db.Employees.ToList();
            foreach (var emp in emps)
            {
                string directlineName = "", dottedlineName = "";
                if (emp.directLine != null)
                {
                    var dirline = emps.FirstOrDefault(e => e.Id == emp.directLine);
                    if (dirline != null)
                    {
                        directlineName = dirline.contact.FullName;
                    }
                }

                if (emp.dottedLine != null)
                {
                    var dotline = emps.FirstOrDefault(e => e.Id == emp.dottedLine);
                    if (dotline != null)
                    {
                        dottedlineName = dotline.contact.FullName;
                    }
                }

                string role = "", workload = "";
                if (emp.Role != null)
                {
                    role = emp.Role.roleName;
                }
                if (emp.Workload != null)
                {
                    workload = emp.Workload1.WorkloadName;
                }

                var empvw = new EmployeesView();
                empvw.Id = emp.Id;
                empvw.BasedOut = emp.contact.BasedOut;
                empvw.Email = emp.contact.Email;
                empvw.HiringDate = emp.hiringDate;
                empvw.Name = emp.contact.FirstName + ' ' + emp.contact.LastName;
                empvw.PhoneNumber = emp.contact.PhoneNumber;
                empvw.VacationBalance = emp.VacationBalance;
                empvw.VacationsCarryOver = emp.VacationsCarryOver;
                empvw.PassportNumber = emp.contact.PassportNumber;
                empvw.Role = role;
                empvw.Workload = workload;
                empvw.DirectLine = directlineName;
                empvw.DottedLine = dottedlineName;

                _db.EmployeesViews.Add(empvw);

                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

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
            ViewBag.EmployeesList = new SelectList(_db.contacts.ToList(), "Id", "FullName");
            ViewBag.RolesList = new SelectList(_db.Roles.ToList(), "Id", "roleName");
            ViewBag.WorkloadsList = new SelectList(_db.Workloads.ToList(), "Id", "WorkloadName");
            ViewBag.CountriesList = new SelectList(_db.Countries.ToList(), "CountryName", "CountryName");
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EmployeeViewModel employeevm)
        {
            var emps = _db.Employees.ToList();
            if (ModelState.IsValid)
            {
                // Add contact
                var contact = new contact()
                {
                    FullName = employeevm.Name,
                    FirstName = employeevm.Name.Split(' ')[0],
                    LastName = employeevm.Name.Split(' ')[1],
                    BasedOut = employeevm.BasedOut.CountryName,
                    UserName = employeevm.UserName,
                    Email = employeevm.UserName + "@microsoft.com",
                    PassportNumber = employeevm.PassportNumber,
                    PhoneNumber = employeevm.PhoneNumber
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
                        VacationsCarryOver = employeevm.VacationsCarryOver,
                        VacationBalance = employeevm.VacationBalance,
                        hiringDate = employeevm.HiringDate,
                        RoleID = employeevm.Role.Id,
                        Workload = employeevm.Workload.Id,
                        Role = roole,
                        Workload1 = woorkload,
                        directLine = employeevm.DirectLine.Id,
                        dottedLine = employeevm.DottedLine.Id
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
                        PassportNumber = contactDb.PassportNumber,
                        VacationBalance = contactDb.Employee.VacationBalance,
                        VacationsCarryOver = contactDb.Employee.VacationsCarryOver,
                        PhoneNumber = contactDb.PhoneNumber,
                        Role = role,
                        Workload = workload,
                        DirectLine = directlineName,
                        DottedLine = dottedlineName
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

            var employeevm = new EmployeeViewModel()
            {
                Id = contact.Id,
                Workload = contact.Employee.Workload1,
                Role = contact.Employee.Role,
                UserName = contact.UserName,
                HiringDate = contact.Employee.hiringDate,
                Name = contact.FullName,
                PassportNumber = contact.PassportNumber,
                PhoneNumber = contact.PhoneNumber,
                VacationBalance = contact.Employee.VacationBalance,
                VacationsCarryOver = contact.Employee.VacationsCarryOver,
                DirectLine = dirline,
                DottedLine = dotline,
                BasedOut = basedOut
            };

            ViewBag.EmployeesList = new SelectList(_db.contacts.ToList(), "Id", "FullName");
            ViewBag.RolesList = new SelectList(_db.Roles.ToList(), "Id", "roleName");
            ViewBag.WorkloadsList = new SelectList(_db.Workloads.ToList(), "Id", "WorkloadName");
            ViewBag.CountriesList = new SelectList(_db.Countries.ToList(), "CountryName", "CountryName");
            return View(employeevm);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EmployeeViewModel employeevm)
        {
            if (ModelState.IsValid)
            {
                //_db.Entry(employee).State = EntityState.Modified;
                //_db.SaveChanges();
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
