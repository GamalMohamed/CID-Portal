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
    public class TripsController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        // GET: Trips
        public ActionResult Index()
        {
            var trips = _db.Trips.Where(t => t.EndDate.Value.Month == DateTime.Now.Month - 1 &&
            t.EndDate.Value.Year == DateTime.Now.Year).ToList();
            var tripsvm = new List<TripViewModel>();
            foreach (var trip in trips)
            {
                // Add trip basic data
                var tripvm = new TripViewModel()
                {
                    Id = trip.Id,
                    EmployeeName = trip.Employee.contact.FullName,
                    StartDate = trip.StartDate,
                    EndDate = trip.EndDate,
                    Country = trip.Routes.ToList()[trip.Routes.Count - 2].Country.CountryName
                };

                if (trip.CashInAdvances.Count > 0 || trip.ExpensesReports.Count > 0)
                {
                    // Fill Trip CIA, plus its Expenses (if exist)
                    foreach (var cia in trip.CashInAdvances)
                    {
                        var tripvm2 = tripvm;
                        tripvm2.CIA_Id = cia.Id;
                        tripvm2.CIA_Status = cia.CashInAdvanceStatu.CashInAdvanceStatus;
                        tripvm2.CurrencyName = cia.Currency.CurrencyName;
                        tripvm2.CIA_Amount_InCurrency = cia.Amount ?? 0;
                        tripvm2.CIA_ExchangeRate = cia.ExchangeRate ?? 0;
                        tripvm2.CIA_Amount_InEGP = tripvm2.CIA_Amount_InCurrency *
                                                       (decimal)tripvm2.CIA_ExchangeRate;
                        tripvm2.CIA_Reason = cia.Reason;
                        tripvm2.OperationsApprovalDate = cia.OperationApprovalDate;


                        if (cia.ExpensesReports.Count > 0)
                        {
                            foreach (var ciaExpensesReport in cia.ExpensesReports)
                            {
                                var tripvm3 = tripvm2;
                                tripvm3.ExpenseReportId = ciaExpensesReport.ID;
                                tripvm3.SubmissionDate = ciaExpensesReport.SubmissionDate;
                                tripvm3.ApprovalDate = ciaExpensesReport.ApprovalDate;
                                tripvm3.ExpenseReportStatus = ciaExpensesReport.ExpenseReportStatu.StatusName;
                                tripvm3.TotalAmountInEGP = ciaExpensesReport.TotalAmountInUSD ?? 0;
                                tripvm3.CIAExpenseReport = ciaExpensesReport.CashInAdvance ?? 0; //(double)tripvm3.CIA_Amount_InEGP;
                                tripvm3.AmountToEmployeeInEGP = (tripvm3.TotalAmountInEGP - tripvm3.CIAExpenseReport) ?? 0;
                                tripvm3.SettledAmount = ciaExpensesReport.SettledAmount ?? 0;
                                tripvm3.SettlementDate = ciaExpensesReport.SettlementDate;
                                tripvm3.RemainingBalance = Math.Abs((decimal)tripvm3.AmountToEmployeeInEGP) - tripvm3.SettledAmount;
                                tripvm3.OperationsComment = ciaExpensesReport.OperationsComment;

                                tripsvm.Add(tripvm3); // Trip with CIA and expenses
                            }
                        }
                        else
                        {
                            tripsvm.Add(tripvm2); // Trip with CIA only
                        }
                    }

                    // Trip expenses without CIA
                    foreach (var expense in trip.ExpensesReports)
                    {
                        if (expense.CashInAdvances.Count == 0)
                        {
                            var tripvm4 = tripvm;
                            tripvm4.ExpenseReportId = expense.ID;
                            tripvm4.SubmissionDate = expense.SubmissionDate;
                            tripvm4.ApprovalDate = expense.ApprovalDate;
                            tripvm4.ExpenseReportStatus = expense.ExpenseReportStatu.StatusName;
                            tripvm4.TotalAmountInEGP = expense.TotalAmountInUSD ?? 0;
                            tripvm4.CIAExpenseReport = expense.CashInAdvance ?? 0;
                            tripvm4.AmountToEmployeeInEGP = (tripvm4.TotalAmountInEGP - tripvm4.CIAExpenseReport) ?? 0;
                            tripvm4.SettledAmount = expense.SettledAmount ?? 0;
                            tripvm4.SettlementDate = expense.SettlementDate;
                            tripvm4.RemainingBalance = (decimal)tripvm4.AmountToEmployeeInEGP - tripvm4.SettledAmount;
                            tripvm4.OperationsComment = expense.OperationsComment;

                            tripsvm.Add(tripvm4); // Trip with Expenses only
                        }
                    }
                }
                else
                {
                    tripsvm.Add(tripvm); //Trip without any CIAs or Expenses
                }
            }

            return View(tripsvm);
        }

        // GET: Trips/Edit/5
        public ActionResult Edit(int? expid, int? ciaId)
        {
            if (expid == null && ciaId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var settlementvm = new SettlementViewModel();
            if (expid == null)
            {
                var cia = _db.CashInAdvances.Find(ciaId);
                if (cia == null)
                {
                    return HttpNotFound();
                }
                settlementvm.CashInAdvance = cia;
                settlementvm.ExpensesReport = null;
            }
            else
            {
                var exp = _db.ExpensesReports.Find(expid);
                if (exp == null)
                {
                    return HttpNotFound();
                }
                settlementvm.ExpensesReport = exp;
            }

            return View(settlementvm);
        }

        // POST: Trips/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SettlementViewModel settlementvm)
        {
            if (ModelState.IsValid)
            {
                if (settlementvm.ExpensesReport == null)
                {
                    var cia = _db.CashInAdvances.Find(settlementvm.CashInAdvance.Id);
                    if (cia == null)
                    {
                        return HttpNotFound();
                    }
                    cia.SettledAmount = settlementvm.CashInAdvance.SettledAmount;
                    cia.OperationsComment = settlementvm.CashInAdvance.OperationsComment;
                }
                else
                {
                    var exp = _db.ExpensesReports.Find(settlementvm.ExpensesReport.ID);
                    if (exp == null)
                    {
                        return HttpNotFound();
                    }
                    exp.SettledAmount = settlementvm.ExpensesReport.SettledAmount;
                    exp.OperationsComment = settlementvm.ExpensesReport.OperationsComment;
                }

                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(settlementvm);
        }

        //// GET: Trips/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    var trip = _db.Trips.Find(id);
        //    if (trip == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    _db.Trips.Remove(trip);
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
