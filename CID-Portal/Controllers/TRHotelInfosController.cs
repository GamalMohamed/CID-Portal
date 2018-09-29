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
    public class TRHotelInfosController : Controller
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

        public List<TRHotelInfoView> SetTrHotelInfoView(List<TRHotelInfo> trHotelInfos)
        {
            var trHotelInfoes = new List<TRHotelInfoView>();
            foreach (var trHotelInfo in trHotelInfos)
            {
                var trHotelInfoV = new TRHotelInfoView
                {
                    Id = trHotelInfo.Id,
                    CheckInDate = trHotelInfo.CheckInDate,
                    CheckOutDate = trHotelInfo.CheckOutDate,
                    TRID = trHotelInfo.TRID,
                    HotelRate = trHotelInfo.HotelRate,
                    Country = trHotelInfo.CountryName,
                    Hotel = trHotelInfo.Hotel?.Name,
                    Currency = trHotelInfo.Currency?.CurrencyName,
                    PaymentMethod = trHotelInfo.PaymentMethod?.MethodName
                };
                if (trHotelInfo.TravelRequest?.Trips?.Count > 0)
                {
                    trHotelInfoV.Name = trHotelInfo.TravelRequest.Trips.ToList()[0].Employee?.contact?.FullName;
                    trHotelInfoes.Add(trHotelInfoV);
                }
            }
            return trHotelInfoes;
        }

        public List<TRHotelInfoView_Archive> SetTrHotelInfoViewArchive(List<TRHotelInfo> trHotelInfos)
        {
            var trHotelInfoes = new List<TRHotelInfoView_Archive>();
            foreach (var trHotelInfo in trHotelInfos)
            {
                var trHotelInfoV = new TRHotelInfoView_Archive()
                {
                    Id = trHotelInfo.Id,
                    CheckInDate = trHotelInfo.CheckInDate,
                    CheckOutDate = trHotelInfo.CheckOutDate,
                    TRID = trHotelInfo.TRID,
                    HotelRate = trHotelInfo.HotelRate,
                    Country = trHotelInfo.CountryName,
                    Hotel = trHotelInfo.Hotel?.Name,
                    Currency = trHotelInfo.Currency?.CurrencyName,
                    PaymentMethod = trHotelInfo.PaymentMethod?.MethodName
                };
                if (trHotelInfo.TravelRequest?.Trips.Count > 0)
                {
                    trHotelInfoV.Name = trHotelInfo.TravelRequest?.Trips.ToList()[0].Employee.contact.FullName;
                    trHotelInfoes.Add(trHotelInfoV);
                }
            }
            return trHotelInfoes;
        }

        public ActionResult FillTRHotelInfoView()
        {
            List<TRHotelInfo> tRHotelInfoes;
            if (DateTime.Now.Month > 6)
            {
                tRHotelInfoes = _db.TRHotelInfoes.Where(h =>
                    (h.CheckInDate.Year == DateTime.Now.Year - 1 && h.CheckInDate.Month > 6) ||
                    (h.CheckInDate.Year == DateTime.Now.Year)
                    ).Include(h => h.Hotel)
                     .Include(h => h.Currency)
                     .Include(h => h.PaymentMethod)
                    .ToList();
            }
            else
            {
                tRHotelInfoes = _db.TRHotelInfoes.Where(h =>
                    (h.CheckInDate.Year == DateTime.Now.Year - 2 && h.CheckInDate.Month > 6) ||
                    (h.CheckInDate.Year == DateTime.Now.Year - 1) ||
                    (h.CheckInDate.Year == DateTime.Now.Year)
                    ).Include(h => h.Hotel)
                     .Include(h => h.Currency)
                     .Include(h => h.PaymentMethod)
                     .ToList();
            }

            var trViews = SetTrHotelInfoView(tRHotelInfoes);
            _db.TRHotelInfoViews.AddRange(trViews);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult FillTRHotelInfoViewArchive()
        {
            var sl = new List<string> { "2016-2017", "2015-2016", "2014-2015", "2013-2014" };
            foreach (var s in sl)
            {
                var seY = s.Split('-'); // e.g. 2018-2019
                var startYear = seY[0];
                var endYear = seY[1];
                var tRHotelInfoes = _db.TRHotelInfoes.Where(t =>
                    (t.CheckInDate.Year.ToString() == startYear && t.CheckInDate.Month > 6) ||
                    (t.CheckInDate.Year.ToString() == endYear && t.CheckInDate.Month < 7))
                     .Include(h => h.Hotel)
                     .Include(h => h.Currency)
                     .Include(h => h.PaymentMethod)
                     .ToList();

                _db.TRHotelInfoView_Archive.AddRange(SetTrHotelInfoViewArchive(tRHotelInfoes));
                _db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // GET: TRHotelInfos
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                var audits = _db.Audits.Where(a => a.Ref_Table == "TRHotelInfo").ToList();

                var auditsClone = new List<Audit>(audits);
                if (audits.Count > 0)
                {
                    foreach (var audit in audits)
                    {
                        if (audit.Operation == "Deleted")
                        {
                            var trHotelInfo = _db.TRHotelInfoViews.Where(h => h.Id == audit.RecordID).ToList();
                            _db.TRHotelInfoViews.RemoveRange(trHotelInfo);
                            _db.SaveChanges();
                        }
                        else if (audit.Operation == "Insert" || audit.Operation == "Update")
                        {
                            if (audit.Operation == "Update")
                            {
                                var trHotelInfoView = _db.TRHotelInfoViews.Where(h => h.Id == audit.RecordID).ToList();
                                _db.TRHotelInfoViews.RemoveRange(trHotelInfoView);
                                _db.SaveChanges();
                            }

                            // Re-insert record
                            var trHotelInfo = _db.TRHotelInfoes.Where(h => h.Id == audit.RecordID).ToList();
                            _db.TRHotelInfoViews.AddRange(SetTrHotelInfoView(trHotelInfo));
                            _db.SaveChanges();
                        }
                    }

                    // Clear the related audit table records after syncing
                    _db.Audits.RemoveRange(auditsClone);
                    _db.SaveChanges();
                }

                return View(_db.TRHotelInfoViews.OrderByDescending(h => h.TRID).ToList());
            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
        }

        // GET: TRHotelInfos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tRHotelInfo = _db.TRHotelInfoes.FirstOrDefault(t => t.TRID == id);
            if (tRHotelInfo == null)
            {
                return HttpNotFound();
            }
            return View(tRHotelInfo);
        }

        // GET: TRHotelInfos/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var tRHotelInfo = _db.TRHotelInfoes.Find(id);
            if (tRHotelInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.Currencies = new SelectList(_db.Currencies.ToList(), "CurrencyName", "CurrencyName");
            ViewBag.PaymentMethods = new SelectList(_db.PaymentMethods.ToList(), "MethodName", "MethodName");
            ViewBag.Hotels = new SelectList(_db.Hotels.ToList(), "Name", "Name");
            return View(tRHotelInfo);
        }

        // POST: TRHotelInfos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TRHotelInfo tRHotelInfo)
        {
            if (ModelState.IsValid)
            {
                var trH = _db.TRHotelInfoes.Find(tRHotelInfo.Id);
                if (trH != null)
                {
                    trH.HotelRate = tRHotelInfo.HotelRate;
                    if (tRHotelInfo.Currency.CurrencyName != null)
                        trH.Currency =
                            _db.Currencies.FirstOrDefault(c => c.CurrencyName == tRHotelInfo.Currency.CurrencyName);
                    else
                        trH.CurrencyID = null;

                    if (tRHotelInfo.PaymentMethod.MethodName != null)
                        trH.PaymentMethod =
                            _db.PaymentMethods.FirstOrDefault(p => p.MethodName == tRHotelInfo.PaymentMethod.MethodName);
                    else
                        trH.PaymentMethodID = null;

                    if (tRHotelInfo.Hotel.Name != null)
                        trH.Hotel = _db.Hotels.FirstOrDefault(h => h.Name == tRHotelInfo.Hotel.Name);
                    else
                        trH.HotelId = null;

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(tRHotelInfo);
        }

        //// GET: TRHotelInfos/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TRHotelInfo tRHotelInfo = db.TRHotelInfoes.Find(id);
        //    if (tRHotelInfo == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    db.TRHotelInfoes.Remove(tRHotelInfo);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}


        public ActionResult Archive(string id)
        {
            if (id != "")
            {
                var seY = id.Split('-'); // e.g. 2018-2019
                if (seY.Length == 2)
                {
                    var startYear = seY[0];
                    var endYear = seY[1];
                    var tRHotelInfoes = _db.TRHotelInfoView_Archive.Where(t =>
                        (t.CheckInDate.Value.Year.ToString() == startYear && t.CheckInDate.Value.Month > 6) ||
                        (t.CheckInDate.Value.Year.ToString() == endYear && t.CheckInDate.Value.Month < 7)
                    ).OrderByDescending(h => h.TRID)
                     .ToList();

                    return View(tRHotelInfoes);
                }
            }
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
