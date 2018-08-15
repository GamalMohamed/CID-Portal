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

        public List<Route> GetRoutesVisas(List<Route> routes)
        {
            var routesV = new List<Route>();
            foreach (var r in routes)
            {
                if (r.Country.CountryName !=
                            r.Trip.Routes.ToList()[r.Trip.Routes.Count - 1].Country.CountryName)
                {
                    routesV.Add(r); // HACK: so as not to return the return route country
                }
            }

            return routesV;
        }

        // GET: Visas
        public ActionResult Index()
        {
            List<Route> routes;
            if (DateTime.Now.Month > 6)
            {
                routes = _db.Routes.Where(h =>
                    ((h.arrivalDate.Value.Year == DateTime.Now.Year - 1 && h.arrivalDate.Value.Month > 6) ||
                    (h.arrivalDate.Value.Year == DateTime.Now.Year)) && h.requireVisa.Value
                    ).ToList();
            }
            else
            {
                routes = _db.Routes.Where(h =>
                    ((h.arrivalDate.Value.Year == DateTime.Now.Year - 2 && h.arrivalDate.Value.Month > 6) ||
                    (h.arrivalDate.Value.Year == DateTime.Now.Year - 1) ||
                    (h.arrivalDate.Value.Year == DateTime.Now.Year)) && h.requireVisa.Value
                    ).ToList();
            }

            return View(GetRoutesVisas(routes));
        }

        // GET: Visas/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var route = _db.Routes.FirstOrDefault(v => v.Trip.TRID == id);
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
            var route = _db.Routes.FirstOrDefault(v => v.routeId == id);
            if (route == null)
            {
                return HttpNotFound();
            }
            ViewBag.Currencies = new SelectList(_db.Currencies.ToList(), "CurrencyName", "CurrencyName");
            return View(route);
        }

        // POST: Visas/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Route route)
        {
            if (ModelState.IsValid)
            {
                var rT = _db.Routes.Find(route.routeId);
                if (rT != null)
                {
                    rT.VisaCost = route.VisaCost;
                    if (route.Currency.CurrencyName != null)
                        rT.Currency =
                            _db.Currencies.FirstOrDefault(c => c.CurrencyName == route.Currency.CurrencyName);
                    else
                        rT.VisaCostCurrencyID = null;

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(route);
        }

        public ActionResult Archive(string id)
        {
            var seY = id.Split('-'); // e.g. 2018-2019
            var startYear = seY[0];
            var endYear = seY[1];
            var routes = _db.Routes.Where(t =>
                    ((t.arrivalDate.Value.Year.ToString() == startYear && t.arrivalDate.Value.Month > 6) ||
                    (t.arrivalDate.Value.Year.ToString() == endYear && t.arrivalDate.Value.Month < 7))
                    && t.requireVisa.Value
                    ).ToList();

            return RedirectToAction("Index", routes);
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
