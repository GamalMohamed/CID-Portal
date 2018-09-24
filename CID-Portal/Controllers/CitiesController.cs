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
    public class CitiesController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        public bool IsAuthorized()
        {
            var loggedUserEmail = "v-gamoha@microsoft.com";
            //var loggedUserEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            var authUser = _db.AuthUsers.FirstOrDefault(u => u.Email == loggedUserEmail);
            if (authUser?.Privilege != null && (Privilege.Admin == (Privilege)authUser.Privilege ||
                                                Privilege.Travel == (Privilege)authUser.Privilege))
            {
                return true;
            }
            return false;
        }

        // GET: Cities
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                var cities = _db.Cities.Include(c => c.Country);
                return View(cities.ToList());
            }
            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        // GET: Cities/Create
        public ActionResult Create()
        {
            ViewBag.CountryID = new SelectList(_db.Countries, "Id", "CountryName");
            return View();
        }

        // POST: Cities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(City city)
        {
            if (ModelState.IsValid)
            {
                _db.Cities.Add(city);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CountryID = new SelectList(_db.Countries, "Id", "CountryName", city.CountryID);
            return View(city);
        }

        // GET: Cities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var city = _db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            ViewBag.CountryID = new SelectList(_db.Countries, "Id", "CountryName", city.CountryID);
            return View(city);
        }

        // POST: Cities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(City city)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(city).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CountryID = new SelectList(_db.Countries, "Id", "CountryName", city.CountryID);
            return View(city);
        }

        // GET: Cities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var city = _db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            _db.Cities.Remove(city);
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
