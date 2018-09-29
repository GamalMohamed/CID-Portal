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
    public class VisasController : Controller
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

        public List<VisasView> SetVisasView(List<Route> routes)
        {
            var visaViews = new List<VisasView>();
            foreach (var route in routes)
            {
                // HACK: so as not to return the return route country
                if (route.Country?.CountryName !=
                            route.Trip?.Routes?.ToList()[route.Trip.Routes.Count - 1].Country?.CountryName)
                {
                    var visaV = new VisasView
                    {
                        Country = route.Country?.CountryName,
                        Currency = route.Currency?.CurrencyName,
                        VisaCost = route.VisaCost,
                        TRID = route.Trip?.TRID,
                        Employee = route.Trip?.Employee?.contact?.FullName,
                        RouteId = route.routeId
                    };
                    visaViews.Add(visaV);
                }
            }

            return visaViews;
        }

        public ActionResult FillVisasView()
        {
            //List<Route> routes;
            //if (DateTime.Now.Month > 6)
            //{
            //    routes = _db.Routes.Where(h =>
            //        ((h.arrivalDate.Value.Year == DateTime.Now.Year - 1 && h.arrivalDate.Value.Month > 6) ||
            //        (h.arrivalDate.Value.Year == DateTime.Now.Year)) && h.requireVisa.Value
            //        ).ToList();
            //}
            //else
            //{
            //    routes = _db.Routes.Where(h =>
            //        ((h.arrivalDate.Value.Year == DateTime.Now.Year - 2 && h.arrivalDate.Value.Month > 6) ||
            //        (h.arrivalDate.Value.Year == DateTime.Now.Year - 1) ||
            //        (h.arrivalDate.Value.Year == DateTime.Now.Year)) && h.requireVisa.Value
            //        ).ToList();
            //}

            var visas = SetVisasView(_db.Routes.Where(r => r.requireVisa.Value)
                                                    .Include(v => v.Trip)
                                                    .Include(v => v.Currency)
                                                    .Include(v=>v.Country)
                                                    .ToList());
            _db.VisasViews.AddRange(visas);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Visas
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                var audits = _db.Audits.Where(a => a.Ref_Table == "Route").ToList();

                var auditsClone = new List<Audit>(audits);
                if (audits.Count > 0)
                {
                    foreach (var audit in audits)
                    {
                        if (audit.Operation == "Deleted")
                        {
                            if (audit.Remark[2] == 'X')
                            {
                                var visasView = _db.VisasViews.Where(v => v.RouteId == audit.RecordID).ToList();
                                if (visasView.Count > 0)
                                {
                                    _db.VisasViews.RemoveRange(visasView);
                                    _db.SaveChanges();

                                    // Sign!
                                    var aud = _db.Audits.Find(audit.Id);
                                    if (aud != null)
                                    {
                                        var remark = aud.Remark;
                                        aud.Remark = remark[0].ToString() + remark[1].ToString() + "V";
                                        _db.SaveChanges();
                                        if (aud.Remark != "TRV")
                                        {
                                            auditsClone.Remove(aud);
                                        }
                                    }
                                }
                            }
                        }
                        else if (audit.Operation == "Insert" || audit.Operation == "Update")
                        {
                            if (audit.Remark[2] == 'X')
                            {
                                if (audit.Operation == "Update")
                                {
                                    var visaView = _db.VisasViews.Where(v => v.RouteId == audit.RecordID).ToList();
                                    _db.VisasViews.RemoveRange(visaView);
                                    _db.SaveChanges();
                                }

                                // Re-insert record
                                var visaRoutes = _db.Routes.Where(h => h.routeId == audit.RecordID).ToList();
                                _db.VisasViews.AddRange(SetVisasView(visaRoutes));
                                _db.SaveChanges();

                                // Sign!
                                var aud = _db.Audits.Find(audit.Id);
                                if (aud != null)
                                {
                                    var remark = aud.Remark;
                                    aud.Remark = remark[0].ToString() + remark[1].ToString() + "V";
                                    _db.SaveChanges();
                                    if (aud.Remark != "TRV")
                                    {
                                        auditsClone.Remove(aud);
                                    }
                                }
                            }
                        }
                    }

                    // Clear the related audit table records after syncing
                    _db.Audits.RemoveRange(auditsClone);
                    _db.SaveChanges();
                }

                return View(_db.VisasViews.ToList());
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
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
                        rT.VisaCostCurrencyID = null; // Kill the relation!

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(route);
        }

        //public ActionResult Archive(string id)
        //{
        //    var seY = id.Split('-'); // e.g. 2018-2019
        //    var startYear = seY[0];
        //    var endYear = seY[1];
        //    var routes = _db.Routes.Where(t =>
        //            ((t.arrivalDate.Value.Year.ToString() == startYear && t.arrivalDate.Value.Month > 6) ||
        //            (t.arrivalDate.Value.Year.ToString() == endYear && t.arrivalDate.Value.Month < 7))
        //            && t.requireVisa.Value
        //            ).ToList();

        //    return RedirectToAction("Index", routes);
        //}

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
