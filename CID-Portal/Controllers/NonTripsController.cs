using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VacationsPortal.Models;

namespace VacationsPortal.Controllers
{
    public class NonTripsController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        // GET: NonTrips
        public ActionResult Index()
        {
            var cashInAdvances = _db.CashInAdvances.Include(c => c.CashInAdvanceStatu).Include(c => c.Currency).Include(c => c.Employee).Include(c => c.Trip).Include(c => c.Employee1).Include(c => c.Employee2);
            return View(cashInAdvances.ToList());
        }

        // GET: NonTrips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashInAdvance cashInAdvance = _db.CashInAdvances.Find(id);
            if (cashInAdvance == null)
            {
                return HttpNotFound();
            }
            ViewBag.CashInAdvanceStatus = new SelectList(_db.CashInAdvanceStatus, "ID", "CashInAdvanceStatus", cashInAdvance.CashInAdvanceStatus);
            ViewBag.CurrencyID = new SelectList(_db.Currencies, "ID", "CurrencyName", cashInAdvance.CurrencyID);
            ViewBag.CreatedBy = new SelectList(_db.Employees, "Id", "employeeId", cashInAdvance.CreatedBy);
            ViewBag.TripID = new SelectList(_db.Trips, "Id", "ModifiedBy", cashInAdvance.TripID);
            ViewBag.OperationApproverID = new SelectList(_db.Employees, "Id", "employeeId", cashInAdvance.OperationApproverID);
            ViewBag.ManagerApproverID = new SelectList(_db.Employees, "Id", "employeeId", cashInAdvance.ManagerApproverID);
            return View(cashInAdvance);
        }

        // POST: NonTrips/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CashInAdvance cashInAdvance)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(cashInAdvance).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CashInAdvanceStatus = new SelectList(_db.CashInAdvanceStatus, "ID", "CashInAdvanceStatus", cashInAdvance.CashInAdvanceStatus);
            ViewBag.CurrencyID = new SelectList(_db.Currencies, "ID", "CurrencyName", cashInAdvance.CurrencyID);
            ViewBag.CreatedBy = new SelectList(_db.Employees, "Id", "employeeId", cashInAdvance.CreatedBy);
            ViewBag.TripID = new SelectList(_db.Trips, "Id", "ModifiedBy", cashInAdvance.TripID);
            ViewBag.OperationApproverID = new SelectList(_db.Employees, "Id", "employeeId", cashInAdvance.OperationApproverID);
            ViewBag.ManagerApproverID = new SelectList(_db.Employees, "Id", "employeeId", cashInAdvance.ManagerApproverID);
            return View(cashInAdvance);
        }

        // GET: NonTrips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            
            CashInAdvance cashInAdvance = _db.CashInAdvances.Find(id);
            if (cashInAdvance == null)
            {
                return HttpNotFound();
            }
            _db.CashInAdvances.Remove(cashInAdvance);
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
