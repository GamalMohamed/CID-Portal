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
    public class NonTripsController : Controller
    {
        private readonly CIDvNEXtEntities _db = new CIDvNEXtEntities();

        // GET: NonTrips
        public ActionResult Index()
        {
            // Get all CIAs and Expenses
            var cashInAdvances = _db.CashInAdvances.Where(c => c.RequestDate.Value.Month == DateTime.Now.Month - 1 &&
            c.RequestDate.Value.Year == DateTime.Now.Year && c.TripID == null).ToList();
            var expenses = _db.ExpensesReports.Where(e => e.SubmissionDate.Month == DateTime.Now.Month - 1 &&
            e.SubmissionDate.Year == DateTime.Now.Year && e.TripID == null).ToList();
            var nonTripsvm = new List<NonTripViewModel>();
            foreach (var cashInAdvance in cashInAdvances)
            {
                var nontripvm = new NonTripViewModel();
                nontripvm.EmployeeName = cashInAdvance.Employee.contact.FullName;
                nontripvm.CIA_Id = cashInAdvance.Id;
                nontripvm.CIA_Status = cashInAdvance.CashInAdvanceStatu.CashInAdvanceStatus;
                nontripvm.CurrencyName = cashInAdvance.Currency.CurrencyName;
                nontripvm.CIA_Amount_InCurrency = cashInAdvance.Amount ?? 0;
                nontripvm.CIA_ExchangeRate = cashInAdvance.ExchangeRate ?? 0;
                nontripvm.CIA_Amount_InEGP = nontripvm.CIA_Amount_InCurrency *
                                                        (decimal)nontripvm.CIA_ExchangeRate;
                nontripvm.CIA_Reason = cashInAdvance.Reason;
                nontripvm.OperationsApprovalDate = cashInAdvance.OperationApprovalDate;

                if (cashInAdvance.ExpensesReports.Count > 0)
                {
                    foreach (var ciaExpensesReport in cashInAdvance.ExpensesReports)
                    {
                        var nontripvm2 = nontripvm;
                        nontripvm2.ExpenseReportId = ciaExpensesReport.ID;
                        nontripvm2.SubmissionDate = ciaExpensesReport.SubmissionDate;
                        nontripvm2.Title = ciaExpensesReport.Title;
                        nontripvm2.ApprovalDate = ciaExpensesReport.ApprovalDate;
                        nontripvm2.ExpenseReportStatus = ciaExpensesReport.ExpenseReportStatu.StatusName;
                        nontripvm2.TotalAmountInEGP = ciaExpensesReport.TotalAmountInUSD ?? 0;
                        nontripvm2.CIAExpenseReport = ciaExpensesReport.CashInAdvance ?? 0; //(double)nontripvm2.CIA_Amount_InEGP;
                        nontripvm2.AmountToEmployeeInEGP = (nontripvm2.TotalAmountInEGP - nontripvm2.CIAExpenseReport) ?? 0;
                        nontripvm2.SettledAmount = ciaExpensesReport.SettledAmount ?? 0;
                        nontripvm2.SettlementDate = ciaExpensesReport.SettlementDate;
                        nontripvm2.RemainingBalance = Math.Abs((decimal)nontripvm2.AmountToEmployeeInEGP) - nontripvm2.SettledAmount;
                        nontripvm2.OperationsComment = ciaExpensesReport.OperationsComment;

                        nonTripsvm.Add(nontripvm2); // CIA + Expenses
                    }
                }
                else
                {
                    nonTripsvm.Add(nontripvm); // CIA only
                }



            }

            // Expenses without CIA
            foreach (var expense in expenses)
            {
                if (expense.CashInAdvances.Count == 0)
                {
                    var nontripvm3 = new NonTripViewModel();
                    nontripvm3.EmployeeName = expense.Employee.contact.FullName;
                    nontripvm3.ExpenseReportId = expense.ID;
                    nontripvm3.Title = expense.Title;
                    nontripvm3.SubmissionDate = expense.SubmissionDate;
                    nontripvm3.ApprovalDate = expense.ApprovalDate;
                    nontripvm3.ExpenseReportStatus = expense.ExpenseReportStatu.StatusName;
                    nontripvm3.TotalAmountInEGP = expense.TotalAmountInUSD ?? 0;
                    nontripvm3.CIAExpenseReport = expense.CashInAdvance ?? 0;
                    nontripvm3.AmountToEmployeeInEGP = (nontripvm3.TotalAmountInEGP - nontripvm3.CIAExpenseReport) ?? 0;
                    nontripvm3.SettledAmount = expense.SettledAmount ?? 0;
                    nontripvm3.SettlementDate = expense.SettlementDate;
                    nontripvm3.RemainingBalance = (decimal)nontripvm3.AmountToEmployeeInEGP - nontripvm3.SettledAmount;
                    nontripvm3.OperationsComment = expense.OperationsComment;

                    nonTripsvm.Add(nontripvm3); // Expense only
                }
            }

            return View(nonTripsvm);
        }

        // GET: NonTrips/Edit/5
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

        // POST: NonTrips/Edit/5
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


        //// GET: NonTrips/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    CashInAdvance cashInAdvance = _db.CashInAdvances.Find(id);
        //    if (cashInAdvance == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    _db.CashInAdvances.Remove(cashInAdvance);
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
