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
    public class CountriesController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        public bool IsAuthorized()
        {
            //var loggedUserEmail = "v-gamoha@microsoft.com";
            var loggedUserEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            var authUser = _db.AuthUsers.FirstOrDefault(u => u.Email == loggedUserEmail);
            if (authUser?.Privilege != null && (Privilege.Admin == (Privilege)authUser.Privilege ||
                                                Privilege.Travel == (Privilege)authUser.Privilege))
            {
                return true;
            }
            return false;
        }


        // GET: Countries
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                var countries = _db.Countries.Include(c => c.Subsidary);
                return View(countries.ToList());
            }
            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        
        // GET: Countries/Create
        public ActionResult Create()
        {
            // ViewBag.SubsidaryID = new SelectList(_db.Subsidaries, "Id", "SubsidaryName");
            return View();
        }

        // POST: Countries/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Country country)
        {
            if (ModelState.IsValid)
            {
                country.Subsidary = _db.Subsidaries.FirstOrDefault(s => s.SubsidaryName == "Others");
                _db.Countries.Add(country);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            // ViewBag.SubsidaryID = new SelectList(_db.Subsidaries, "Id", "SubsidaryName", country.SubsidaryID);
            return View(country);
        }

        // GET: Countries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var country = _db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }

            // ViewBag.SubsidaryID = new SelectList(_db.Subsidaries, "Id", "SubsidaryName", country.SubsidaryID);
            return View(country);
        }

        // POST: Countries/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Country country)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(country).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.SubsidaryID = new SelectList(_db.Subsidaries, "Id", "SubsidaryName", country.SubsidaryID);
            return View(country);
        }

        // GET: Countries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var country = _db.Countries.Find(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            _db.Countries.Remove(country);
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
