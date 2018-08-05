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
    public class TripsController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        // GET: Trips
        public ActionResult Index()
        {
            var trips = _db.Trips.Where(t => t.EndDate.Value.Month == DateTime.Now.Month - 1 &&
            t.EndDate.Value.Year == DateTime.Now.Year).ToList();
            return View(trips);
        }

        // GET: Trips/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = _db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            return View(trip);
        }

        // GET: Trips/Create
        public ActionResult Create()
        {
            ViewBag.EmployeeId = new SelectList(_db.Employees, "Id", "employeeId");
            ViewBag.TRID = new SelectList(_db.TravelRequests, "TRID", "RequesterNotes");
            return View();
        }

        // POST: Trips/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,EmployeeId,StartDate,EndDate,NumberofDays,NumberofWorkingDays,Status,ModifiedBy,ModifiedOn,Business,IsCanceled,TRID")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                _db.Trips.Add(trip);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.EmployeeId = new SelectList(_db.Employees, "Id", "employeeId", trip.EmployeeId);
            ViewBag.TRID = new SelectList(_db.TravelRequests, "TRID", "RequesterNotes", trip.TRID);
            return View(trip);
        }

        // GET: Trips/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Trip trip = _db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            ViewBag.EmployeeId = new SelectList(_db.Employees, "Id", "employeeId", trip.EmployeeId);
            ViewBag.TRID = new SelectList(_db.TravelRequests, "TRID", "RequesterNotes", trip.TRID);
            return View(trip);
        }

        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,EmployeeId,StartDate,EndDate,NumberofDays,NumberofWorkingDays,Status,ModifiedBy,ModifiedOn,Business,IsCanceled,TRID")] Trip trip)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(trip).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.EmployeeId = new SelectList(_db.Employees, "Id", "employeeId", trip.EmployeeId);
            ViewBag.TRID = new SelectList(_db.TravelRequests, "TRID", "RequesterNotes", trip.TRID);
            return View(trip);
        }

        // GET: Trips/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var trip = _db.Trips.Find(id);
            if (trip == null)
            {
                return HttpNotFound();
            }
            _db.Trips.Remove(trip);
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
