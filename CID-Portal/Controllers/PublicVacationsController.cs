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
    public class PublicVacationsController : Controller
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

        // GET: PublicVacations
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                return View(_db.PublicVacations.Where(v => v.StartDate.Year == DateTime.Now.Year ||
                                                           v.EndDate.Year == DateTime.Now.Year)
                                                           .ToList()
                                                           .OrderByDescending(v => v.StartDate)
                                                           .ThenByDescending(v => v.EndDate));
            }
            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        // GET: PublicVacations/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PublicVacations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PublicVacation publicVacation)
        {
            if (ModelState.IsValid)
            {
                _db.PublicVacations.Add(publicVacation);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(publicVacation);
        }

        // GET: PublicVacations/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var publicVacation = _db.PublicVacations.Find(id);
            if (publicVacation == null)
            {
                return HttpNotFound();
            }
            return View(publicVacation);
        }

        // POST: PublicVacations/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(PublicVacation publicVacation)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(publicVacation).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(publicVacation);
        }

        // GET: PublicVacations/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var publicVacation = _db.PublicVacations.Find(id);
            if (publicVacation == null)
            {
                return HttpNotFound();
            }
            _db.PublicVacations.Remove(publicVacation);
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
