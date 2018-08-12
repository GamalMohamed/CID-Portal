using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VacationsPortal.Models;
using VacationsPortal.ViewModels;

namespace VacationsPortal.Controllers
{
    public class TravelRequestsController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        // GET: TravelRequests
        public ActionResult Index()
        {
            var travelRequests = _db.TravelRequests.Where(
                 t => t.RequestedOn.Month == DateTime.Now.Month - 1 &&
                 t.RequestedOn.Year == DateTime.Now.Year).ToList();

            var statusesDic = _db.TRItemsStatus.ToDictionary(status => status.Id, status => status.Status);

            var travelRequestsvm = new List<TravelRequestViewModel>();
            foreach (var travelRequest in travelRequests)
            {
                foreach (var trip in travelRequest.Trips.ToList())
                {
                    var tRvm = new TravelRequestViewModel();
                    tRvm.Id = travelRequest.TRID;
                    tRvm.EmployeeName = trip.Employee.contact.FullName;
                    tRvm.StartDate = trip.StartDate;
                    tRvm.EndDate = trip.EndDate;
                    tRvm.NumOfDays = trip.NumberofDays;
                    tRvm.WorkingDays = trip.NumberofWorkingDays;
                    tRvm.ModifiedOn = trip.ModifiedOn;
                    tRvm.IsCanceled = trip.IsCanceled;
                    tRvm.Routes = trip.Routes.ToList()[0].City.Name;
                    for (int i = 1; i < trip.Routes.Count; i++)
                    {
                        tRvm.Routes += ',' + trip.Routes.ToList()[i].City.Name;
                    }
                    tRvm.TRStatus = travelRequest.TravelRequestStatu.Status;
                    if (travelRequest.FlightStatus != null)
                    {
                        if (statusesDic.ContainsKey((int)travelRequest.FlightStatus))
                        {
                            tRvm.FlightStatus = statusesDic[(int)travelRequest.FlightStatus];
                        }
                    }
                    if (travelRequest.HotelStatus != null)
                    {
                        if (statusesDic.ContainsKey((int)travelRequest.HotelStatus))
                        {
                            tRvm.HotelStatus = statusesDic[(int)travelRequest.HotelStatus];
                        }
                    }
                    if (travelRequest.VisaStatus != null)
                    {
                        if (statusesDic.ContainsKey((int)travelRequest.VisaStatus))
                        {
                            tRvm.VisaStatus = statusesDic[(int)travelRequest.VisaStatus];
                        }
                    }
                    if (travelRequest.HRLetterStatus != null)
                    {
                        if (statusesDic.ContainsKey((int)travelRequest.HRLetterStatus))
                        {
                            tRvm.HRLetterStatus = statusesDic[(int)travelRequest.HRLetterStatus];
                        }
                    }

                    tRvm.FlightCost = travelRequest.TotalFlightCost;
                    if (travelRequest.Currency != null)
                    {
                        tRvm.FlightCostCurrency = travelRequest.Currency.CurrencyName;
                    }
                    if (travelRequest.Currency2 != null)
                    {
                        tRvm.FlightCost2Currency = travelRequest.Currency2.CurrencyName;
                    }
                    if (travelRequest.Currency1 != null)
                    {
                        tRvm.HotelCostCurrency = travelRequest.Currency1.CurrencyName;
                    }

                    tRvm.FlightCost2 = travelRequest.TotalFlightCost2;
                    tRvm.ReissueCost = travelRequest.TotalReissueCost;
                    tRvm.ReissueCost2 = travelRequest.TotalReissueCost2;
                    tRvm.ReissueCost3 = travelRequest.TotalReissueCost3;
                    tRvm.FlightRefund = travelRequest.FlightRefundCost;
                    tRvm.HotelCost = travelRequest.TotalHotelCost;
                    tRvm.RequesterNotes = travelRequest.RequesterNotes;
                    tRvm.OperationsNotes = travelRequest.OperationsNotes;
                    tRvm.TripType = trip.Business != null && (bool)trip.Business ? "Business" : "Training";

                    travelRequestsvm.Add(tRvm);
                }
            }

            return View(travelRequestsvm);
        }

        // GET: TravelRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TravelRequest travelRequest = _db.TravelRequests.Find(id);
            if (travelRequest == null)
            {
                return HttpNotFound();
            }

            return View(travelRequest);
        }

        // POST: TravelRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TravelRequest travelRequest)
        {
            if (ModelState.IsValid)
            {
                _db.Entry(travelRequest).State = EntityState.Modified;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(travelRequest);
        }

        //// GET: TravelRequests/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    TravelRequest travelRequest = _db.TravelRequests.Find(id);
        //    if (travelRequest == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    _db.TravelRequests.Remove(travelRequest);
        //    _db.SaveChanges();
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
