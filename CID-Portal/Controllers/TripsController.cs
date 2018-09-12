﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Web;
using System.Web.Mvc;
using VacationsPortal.Models;
using VacationsPortal.ViewModels;

namespace VacationsPortal.Controllers
{
    public class TripsController : Controller
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

        // Form the Trips View list
        public List<TripsView> SetTripsViewList(List<Trip> trips)
        {
            var tripsvm = new List<TripsView>();
            foreach (var trip in trips)
            {
                // Add trip basic data
                var tripvm = new TripsView()
                {
                    TripID = trip.Id,
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
            return tripsvm;
        }

        public List<TripsView_Archive> SetTripsViewListArchive(List<Trip> trips)
        {
            var tripsvm = new List<TripsView_Archive>();
            foreach (var trip in trips)
            {
                // Add trip basic data
                var tripvm = new TripsView_Archive()
                {
                    TripID = trip.Id,
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
            return tripsvm;
        }

        // IMP NOTE: THESE ROUTES ARE FOR DEVELOPMENT PURPOSES ONLY!!
        //public ActionResult FillTripsView()
        //{
        //    List<Trip> trips;
        //    if (DateTime.Now.Month > 6)
        //    {
        //        trips = _db.Trips.Where(t =>
        //            (t.StartDate.Value.Year == DateTime.Now.Year - 1 && t.StartDate.Value.Month > 6) ||
        //            (t.StartDate.Value.Year == DateTime.Now.Year)
        //            ).ToList();
        //    }
        //    else
        //    {
        //        trips = _db.Trips.Where(t =>
        //            (t.StartDate.Value.Year == DateTime.Now.Year - 2 && t.StartDate.Value.Month > 6) ||
        //            (t.StartDate.Value.Year == DateTime.Now.Year - 1) ||
        //            (t.StartDate.Value.Year == DateTime.Now.Year)
        //            ).ToList();
        //    }

        //    var tripViews = SetTripsViewList(trips.OrderByDescending(t => t.StartDate).ToList());
        //    _db.TripsViews.AddRange(tripViews);
        //    _db.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        //public ActionResult FillTripsViewArchive()
        //{
        //    var s = "2013-2014";
        //    var seY = s.Split('-'); // e.g. 2018-2019
        //    var startYear = seY[0];
        //    var endYear = seY[1];
        //    var trips = _db.Trips.Where(t =>
        //            (t.StartDate.Value.Year.ToString() == startYear && t.StartDate.Value.Month > 6) ||
        //            (t.StartDate.Value.Year.ToString() == endYear && t.StartDate.Value.Month < 7)
        //            ).ToList();

        //    var tripViews = SetTripsViewListArchive(trips.OrderByDescending(t => t.StartDate).ToList());
        //    _db.TripsView_Archive.AddRange(tripViews);
        //    _db.SaveChanges();

        //    return RedirectToAction("Index");
        //}

        // GET: Trips
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                // 1. Get new Trips IDs added from the audit table
                var audits = _db.Audits.Where(a => a.Ref_Table == "Trips").ToList();
                List<TripsView> tripsView;

                // SYNCING UPDATES: If the audits isn't empty, then new Trips are either added, deleted or updated
                if (audits.Count > 0)
                {
                    // 2. Get each Trip record from the Trips table
                    foreach (var audit in audits)
                    {
                        if (audit.Operation == "Deleted")
                        {
                            var tripV = _db.TripsViews.Where(t => t.TripID == audit.RecordID).ToList();
                            if (tripV.Count > 0)
                            {
                                _db.TripsViews.RemoveRange(tripV);
                                _db.SaveChanges();
                            }
                        }
                        else if (audit.Operation == "Insert" || audit.Operation == "Update")
                        {
                            if (audit.Operation == "Update")
                            {
                                var tripV = _db.TripsViews.Where(t => t.TripID == audit.RecordID).ToList();
                                if (tripV.Count > 0)
                                {
                                    _db.TripsViews.RemoveRange(tripV);
                                    _db.SaveChanges();
                                }
                            }
                            var trip = _db.Trips.Where(a => a.Id == audit.RecordID).ToList();
                            if (trip.Count > 0)
                            {
                                _db.TripsViews.AddRange(SetTripsViewList(trip));
                                _db.SaveChanges();
                            }
                        }
                    }
                    // Clear the related audit table records after syncing
                    _db.Audits.RemoveRange(audits);
                    _db.SaveChanges();
                }

                if (DateTime.Now.Month > 6)
                {
                    tripsView = _db.TripsViews.Where(t =>
                        (t.StartDate.Value.Year == DateTime.Now.Year - 1 && t.StartDate.Value.Month > 6) ||
                        (t.StartDate.Value.Year == DateTime.Now.Year)
                        ).ToList();
                }
                else
                {
                    tripsView = _db.TripsViews.Where(t =>
                        (t.StartDate.Value.Year == DateTime.Now.Year - 2 && t.StartDate.Value.Month > 6) ||
                        (t.StartDate.Value.Year == DateTime.Now.Year - 1) ||
                        (t.StartDate.Value.Year == DateTime.Now.Year)
                        ).ToList();
                }

                //tripsView = _db.TripsViews.Where(t =>
                //t.EndDate.Value.Month == DateTime.Now.Month - 1 &&
                //t.EndDate.Value.Year == DateTime.Now.Year).ToList();

                return View(tripsView); // If nothing new added, just return the TripsView as it is

            }

            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
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

        public ActionResult Archive(string id)
        {
            var seY = id.Split('-'); // e.g. 2018-2019
            var startYear = seY[0];
            var endYear = seY[1];
            var trips = _db.TripsView_Archive.Where(t =>
                    (t.StartDate.Value.Year.ToString() == startYear && t.StartDate.Value.Month > 6) ||
                    (t.StartDate.Value.Year.ToString() == endYear && t.StartDate.Value.Month < 7)
                    ).ToList();

            return View(trips);
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
