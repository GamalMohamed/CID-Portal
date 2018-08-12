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
    public class VisasController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        // GET: Visas
        public ActionResult Index()
        {
            var routes = _db.Routes.Where(
                t => t.arrivalDate.Value.Year == DateTime.Now.Year).ToList();
            var routesV = new List<Route>();
            foreach (var r in routes)
            {
                if (r.Country.CountryName !=
                            r.Trip.Routes.ToList()[r.Trip.Routes.Count - 1].Country.CountryName)
                {
                    routesV.Add(r);
                }
            }

            return View(routesV);
        }

        // GET: Visas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Route route = _db.Routes.Find(id);
            if (route == null)
            {
                return HttpNotFound();
            }
            return View(route);
        }

        // GET: Visas/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Route route = _db.Routes.Find(id);
            if (route == null)
            {
                return HttpNotFound();
            }
            
            return View(route);
        }

        // POST: Visas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Route route)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(route).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(route);
        }

        //// GET: Visas/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Route route = db.Routes.Find(id);
        //    if (route == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    db.Routes.Remove(route);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

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
