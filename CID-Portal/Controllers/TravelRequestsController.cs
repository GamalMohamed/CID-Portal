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

        public TravelRequestViewModel SetTRvmFields(TravelRequest travelRequest, Trip trip,
                                                    Dictionary<int, string> statusesDic)
        {
            var tRvm = new TravelRequestViewModel
            {
                Id = travelRequest.TRID,
                TRStatus = travelRequest.TravelRequestStatu.Status,
                FlightCost = travelRequest.TotalFlightCost,
                FlightCost2 = travelRequest.TotalFlightCost2,
                ReissueCost = travelRequest.TotalReissueCost,
                ReissueCost2 = travelRequest.TotalReissueCost2,
                ReissueCost3 = travelRequest.TotalReissueCost3,
                FlightRefund = travelRequest.FlightRefundCost,
                HotelCost = travelRequest.TotalHotelCost,
                RequesterNotes = travelRequest.RequesterNotes,
                OperationsNotes = travelRequest.OperationsNotes,
                TripType = trip.Business != null && (bool)trip.Business ? "Business" : "Training",
                EmployeeName = trip.Employee.contact.FullName,
                StartDate = trip.StartDate,
                EndDate = trip.EndDate,
                NumOfDays = trip.NumberofDays,
                WorkingDays = trip.NumberofWorkingDays,
                ModifiedOn = trip.ModifiedOn,
                IsCanceled = trip.IsCanceled,
                Routes = trip.Routes.ToList()[0].City.Name
            };

            for (var i = 1; i < trip.Routes.Count; i++)
            {
                tRvm.Routes += ',' + trip.Routes.ToList()[i].City.Name;
            }
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

            if (travelRequest.FlightInvoiceStatus != null)
            {
                tRvm.FlightInvoiceStatus = travelRequest.FlightInvoiceStatus == 0 ? "Not Invoiced" : "Invoiced";
            }
            if (travelRequest.FlightInvoiceStatus2 != null)
            {
                tRvm.FlightInvoiceStatus2 = travelRequest.FlightInvoiceStatus2 == 0 ? "Not Invoiced" : "Invoiced";
            }
            if (travelRequest.FlightRefundInvoiceStatus != null)
            {
                tRvm.FlightRefundInvoiceStatus = travelRequest.FlightRefundInvoiceStatus == 0 ? "Not Invoiced" : "Invoiced";
            }
            if (travelRequest.HoteInvoiceStatus != null)
            {
                tRvm.HotelInvoiceStatus = travelRequest.HoteInvoiceStatus == 0 ? "Not Invoiced" : "Invoiced";
            }
            if (travelRequest.VisaInvoiceStatus != null)
            {
                tRvm.VisaInvoiceStatus = travelRequest.VisaInvoiceStatus == 0 ? "Not Invoiced" : "Invoiced";
            }
            if (travelRequest.TotalReissueStatus != null)
            {
                tRvm.ReissueInvoiceStatus = travelRequest.TotalReissueStatus == 0 ? "Not Invoiced" : "Invoiced";
            }
            if (travelRequest.TotalReissueStatus2 != null)
            {
                tRvm.ReissueInvoiceStatus2 = travelRequest.TotalReissueStatus2 == 0 ? "Not Invoiced" : "Invoiced";
            }
            if (travelRequest.TotalReissueStatus3 != null)
            {
                tRvm.ReissueInvoiceStatus3 = travelRequest.TotalReissueStatus3 == 0 ? "Not Invoiced" : "Invoiced";
            }

            return tRvm;
        }

        public List<TravelRequestViewModel> SetTRvmList(List<TravelRequest> travelRequests)
        {
            var statusesDic = _db.TRItemsStatus.ToDictionary(status => status.Id, status => status.Status);
            var travelRequestsvm = new List<TravelRequestViewModel>();
            foreach (var travelRequest in travelRequests)
            {
                foreach (var trip in travelRequest.Trips.ToList())
                {
                    travelRequestsvm.Add(SetTRvmFields(travelRequest, trip, statusesDic));
                }
            }
            return travelRequestsvm;
        }

        // GET: TravelRequests
        public ActionResult Index()
        {
            List<TravelRequest> travelRequests;
            if (DateTime.Now.Month > 6)
            {
                travelRequests = _db.TravelRequests.Where(t =>
                    (t.RequestedOn.Year == DateTime.Now.Year - 1 && t.RequestedOn.Month > 6) ||
                    (t.RequestedOn.Year == DateTime.Now.Year)
                    ).ToList();
            }
            else
            {
                travelRequests = _db.TravelRequests.Where(t =>
                    (t.RequestedOn.Year == DateTime.Now.Year - 2 && t.RequestedOn.Month > 6) ||
                    (t.RequestedOn.Year == DateTime.Now.Year - 1) ||
                    (t.RequestedOn.Year == DateTime.Now.Year)
                    ).ToList();
            }

            return View(SetTRvmList(travelRequests));
        }

        // GET: TravelRequests/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var travelRequest = _db.TravelRequests.Find(id);
            if (travelRequest == null)
            {
                return HttpNotFound();
            }
            var statuses = _db.TRItemsStatus;
            var statusesDic = statuses.ToDictionary(status => status.Id, status => status.Status);
            var tRvm = SetTRvmFields(travelRequest, travelRequest.Trips.ToList()[0], statusesDic);
            ViewBag.Statuses = new SelectList(statuses.ToList(), "Status", "Status");
            ViewBag.TRStatuses = new SelectList(_db.TravelRequestStatus.ToList(), "Status", "Status");
            ViewBag.Currencies = new SelectList(_db.Currencies.ToList(), "CurrencyName", "CurrencyName");
            ViewBag.InvoiceStatuses = new SelectList(new List<string>() { "Not Invoiced", "Invoiced" });
            return View(tRvm);
        }

        // POST: TravelRequests/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TravelRequestViewModel travelRequestvm)
        {
            if (ModelState.IsValid)
            {
                var travelRequest = _db.TravelRequests.Find(travelRequestvm.Id);
                if (travelRequest != null)
                {
                    var TRstatuses = _db.TravelRequestStatus.ToList();
                    travelRequest.TravelRequestStatu = TRstatuses.FirstOrDefault(t => t.Status == travelRequestvm.TRStatus);

                    var currencies = _db.Currencies.ToList();
                    travelRequest.Currency = currencies.FirstOrDefault(t => t.CurrencyName == travelRequestvm.FlightCostCurrency);
                    travelRequest.Currency2 = currencies.FirstOrDefault(t => t.CurrencyName == travelRequestvm.FlightCost2Currency);
                    travelRequest.Currency1 = currencies.FirstOrDefault(t => t.CurrencyName == travelRequestvm.HotelCostCurrency);

                    var statuses = _db.TRItemsStatus.ToList();
                    var s1 = statuses.FirstOrDefault(s => s.Status == travelRequestvm.VisaStatus);
                    if (s1 != null)
                        travelRequest.VisaStatus = s1.Id;
                    var s2 = statuses.FirstOrDefault(s => s.Status == travelRequestvm.HotelStatus);
                    if (s2 != null)
                        travelRequest.HotelStatus = s2.Id;
                    var s3 = statuses.FirstOrDefault(s => s.Status == travelRequestvm.FlightStatus);
                    if (s3 != null)
                        travelRequest.FlightStatus = s3.Id;
                    var s4 = statuses.FirstOrDefault(s => s.Status == travelRequestvm.HRLetterStatus);
                    if (s4 != null)
                        travelRequest.HRLetterStatus = s4.Id;

                    travelRequest.TotalFlightCost = travelRequestvm.FlightCost;
                    travelRequest.TotalFlightCost2 = travelRequestvm.FlightCost2;
                    travelRequest.TotalHotelCost = travelRequestvm.HotelCost;
                    travelRequest.OperationsNotes = travelRequestvm.OperationsNotes;
                    travelRequest.TotalReissueCost = travelRequestvm.ReissueCost;
                    travelRequest.TotalReissueCost2 = travelRequestvm.ReissueCost2;
                    travelRequest.TotalReissueCost3 = travelRequestvm.ReissueCost3;
                    travelRequest.FlightRefundCost = travelRequestvm.FlightRefund;

                    if (travelRequestvm.FlightInvoiceStatus != null)
                        travelRequest.FlightInvoiceStatus = travelRequestvm.FlightInvoiceStatus == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.FlightInvoiceStatus = null;

                    if (travelRequestvm.FlightInvoiceStatus2 != null)
                        travelRequest.FlightInvoiceStatus2 = travelRequestvm.FlightInvoiceStatus2 == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.FlightInvoiceStatus2 = null;

                    if (travelRequestvm.ReissueInvoiceStatus != null)
                        travelRequest.TotalReissueStatus = travelRequestvm.ReissueInvoiceStatus == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.TotalReissueStatus = null;

                    if (travelRequestvm.ReissueInvoiceStatus2 != null)
                        travelRequest.TotalReissueStatus2 = travelRequestvm.ReissueInvoiceStatus2 == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.TotalReissueStatus2 = null;

                    if (travelRequestvm.ReissueInvoiceStatus3 != null)
                        travelRequest.TotalReissueStatus3 = travelRequestvm.ReissueInvoiceStatus3 == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.TotalReissueStatus3 = null;

                    if (travelRequestvm.HotelInvoiceStatus != null)
                        travelRequest.HoteInvoiceStatus = travelRequestvm.HotelInvoiceStatus == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.HoteInvoiceStatus = null;

                    if (travelRequestvm.VisaInvoiceStatus != null)
                        travelRequest.VisaInvoiceStatus = travelRequestvm.VisaInvoiceStatus == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.VisaInvoiceStatus = null;

                    if (travelRequestvm.FlightRefundInvoiceStatus != null)
                        travelRequest.FlightRefundInvoiceStatus = travelRequestvm.FlightRefundInvoiceStatus == "Invoiced" ? 1 : 0;
                    else
                        travelRequest.FlightRefundInvoiceStatus = null;

                    _db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(travelRequestvm);
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

        public ActionResult Archive(string id)
        {
            var seY = id.Split('-'); // e.g. 2018-2019
            var startYear = seY[0];
            var endYear = seY[1];
            var travelRequests = _db.TravelRequests.Where(t =>
                    ((t.RequestedOn.Year.ToString() == startYear && t.RequestedOn.Month > 6) ||
                    (t.RequestedOn.Year.ToString() == endYear && t.RequestedOn.Month < 7))
                    ).ToList();

            return View("Index", SetTRvmList(travelRequests));
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
