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
            var tRHotelInfoes = _db.TRHotelInfoes.Include(t => t.Currency).Include(t => t.Hotel).Include(t => t.TravelRequest);
            return View(tRHotelInfoes.ToList());
        }

        // GET: TRHotelInfos/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TRHotelInfo tRHotelInfo = _db.TRHotelInfoes.Find(id);
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
            TRHotelInfo tRHotelInfo = _db.TRHotelInfoes.Find(id);
            if (tRHotelInfo == null)
            {
                return HttpNotFound();
            }
            ViewBag.CurrencyID = new SelectList(_db.Currencies, "ID", "CurrencyName", tRHotelInfo.CurrencyID);
            ViewBag.HotelId = new SelectList(_db.Hotels, "Id", "Name", tRHotelInfo.HotelId);
            ViewBag.TRID = new SelectList(_db.TravelRequests, "TRID", "RequesterNotes", tRHotelInfo.TRID);
            return View(tRHotelInfo);
        }

        // POST: TRHotelInfos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TRHotelInfo tRHotelInfo)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(tRHotelInfo).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CurrencyID = new SelectList(_db.Currencies, "ID", "CurrencyName", tRHotelInfo.CurrencyID);
            ViewBag.HotelId = new SelectList(_db.Hotels, "Id", "Name", tRHotelInfo.HotelId);
            ViewBag.TRID = new SelectList(_db.TravelRequests, "TRID", "RequesterNotes", tRHotelInfo.TRID);
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
