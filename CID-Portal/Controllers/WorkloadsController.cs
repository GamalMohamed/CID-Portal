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
    public class WorkloadsController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        public bool IsAuthorized()
        {
            var loggedUserEmail = "v-gamoha@microsoft.com";
            //var loggedUserEmail = ClaimsPrincipal.Current.FindFirst(ClaimTypes.Name).Value;
            var authUser = _db.AuthUsers.FirstOrDefault(u => u.Email == loggedUserEmail);
            if (authUser?.Privilege != null && (Privilege.Admin == (Privilege)authUser.Privilege ||
                                                Privilege.Vacations == (Privilege)authUser.Privilege))
            {
                return true;
            }
            return false;
        }

        // GET: Workloads
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                return View(_db.Workloads.ToList());
            }
            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        // GET: Workloads/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Workloads/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Workload workload)
        {
            if (ModelState.IsValid)
            {
                _db.Workloads.Add(workload);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workload);
        }

        // GET: Workloads/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var workload = _db.Workloads.Find(id);
            if (workload == null)
            {
                return HttpNotFound();
            }
            return View(workload);
        }

        // POST: Workloads/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Workload workload)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(workload).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(workload);
        }

        // GET: Workloads/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var workload = _db.Workloads.Find(id);
            if (workload == null)
            {
                return HttpNotFound();
            }
            _db.Workloads.Remove(workload);
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
