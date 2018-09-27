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
    public class AuthUsersController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        public bool IsAuthorized()
        {
            //var loggedUserEmail = "v-gamoha@microsoft.com";
            var loggedUserEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            var authUser = _db.AuthUsers.FirstOrDefault(u => u.Email == loggedUserEmail);
            if (authUser?.Privilege != null && Privilege.Admin == (Privilege)authUser.Privilege)
            {
                return true;
            }
            return false;
        }

        // GET: AuthUsers
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                return View(_db.AuthUsers.ToList());
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        
        // GET: AuthUsers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AuthUsers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(AuthUser authUser)
        {
            if (ModelState.IsValid)
            {
                _db.AuthUsers.Add(authUser);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(authUser);
        }

        // GET: AuthUsers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var authUser = _db.AuthUsers.Find(id);
            if (authUser == null)
            {
                return HttpNotFound();
            }

            return View(authUser);
        }

        // POST: AuthUsers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AuthUser authUser)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(authUser).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(authUser);
        }

        // GET: AuthUsers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var authUser = _db.AuthUsers.Find(id);
            if (authUser == null)
            {
                return HttpNotFound();
            }
            _db.AuthUsers.Remove(authUser);
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
