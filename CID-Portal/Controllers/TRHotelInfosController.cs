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
    public class TRHotelInfosController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        // GET: TRHotelInfos
        public ActionResult Index()
        {
            var tRHotelInfoes = _db.TRHotelInfoes.Where(
                t => t.CheckOutDate.Month == DateTime.Now.Month - 1 &&
                 t.CheckOutDate.Year == DateTime.Now.Year).ToList();
            return View(tRHotelInfoes);
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
