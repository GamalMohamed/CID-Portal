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
    public class CurrenciesController : Controller
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

        // GET: Currencies
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                return View(_db.Currencies.ToList());
            }
            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        // GET: Currencies/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Currencies/Create
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create(Currency currency)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _db.Currencies.Add(currency);
        //        _db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(currency);
        //}

        // GET: Currencies/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currency = _db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            return View(currency);
        }

        // POST: Currencies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Currency currency)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(currency).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(currency);
        }

        // GET: Currencies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var currency = _db.Currencies.Find(id);
            if (currency == null)
            {
                return HttpNotFound();
            }
            _db.Currencies.Remove(currency);
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
