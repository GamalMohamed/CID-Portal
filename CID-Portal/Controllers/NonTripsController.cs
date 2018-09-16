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

        public List<NonTripsView> SetNonTripsViewList(List<CashInAdvance> cashInAdvances, List<ExpensesReport> expenses)
        {
            var nonTripsvm = new List<NonTripsView>();
            foreach (var cashInAdvance in cashInAdvances)
            {
                var nontripvm = new NonTripsView();
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
                    var nontripvm3 = new NonTripsView();
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

            return nonTripsvm;
        }

        // IMP NOTE: THESE ROUTES ARE FOR DEVELOPMENT PURPOSES ONLY!!
        public ActionResult FillNonTripsView()
        {
            // Get all CIAs and Expenses
            List<CashInAdvance> cashInAdvances;
            List<ExpensesReport> expenses;
            if (DateTime.Now.Month > 6)
            {
                cashInAdvances = _db.CashInAdvances.Where(c =>
                    ((c.RequestDate.Value.Year == DateTime.Now.Year - 1 && c.RequestDate.Value.Month > 6) ||
                    (c.RequestDate.Value.Year == DateTime.Now.Year)) && c.TripID == null
                    ).ToList();
                expenses = _db.ExpensesReports.Where(e =>
                    ((e.SubmissionDate.Year == DateTime.Now.Year - 1 && e.SubmissionDate.Month > 6) ||
                    (e.SubmissionDate.Year == DateTime.Now.Year)) && e.TripID == null
                    ).ToList();
            }
            else
            {
                cashInAdvances = _db.CashInAdvances.Where(c =>
                    ((c.RequestDate.Value.Year == DateTime.Now.Year - 2 && c.RequestDate.Value.Month > 6) ||
                    (c.RequestDate.Value.Year == DateTime.Now.Year - 1) ||
                    (c.RequestDate.Value.Year == DateTime.Now.Year)) && c.TripID == null
                    ).ToList();

                expenses = _db.ExpensesReports.Where(e =>
                    ((e.SubmissionDate.Year == DateTime.Now.Year - 2 && e.SubmissionDate.Month > 6) ||
                    (e.SubmissionDate.Year == DateTime.Now.Year - 1) ||
                    (e.SubmissionDate.Year == DateTime.Now.Year)) && e.TripID == null
                    ).ToList();
            }

            var nonTripViews = SetNonTripsViewList(
                cashInAdvances.OrderByDescending(c => c.RequestDate).ToList(),
                expenses.OrderByDescending(e => e.SubmissionDate).ToList());

            _db.NonTripsViews.AddRange(nonTripViews);
            _db.SaveChanges();

            return RedirectToAction("Index","Employees");
        }

        // GET: NonTrips
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                var audits = _db.Audits.Where(a => a.Ref_Table == "CIA" ||
                                              a.Ref_Table == "ExpensesReport").ToList();

                // SYNCING UPDATES: If the audits isn't empty, then new records are either added, deleted or updated
                if (audits.Count > 0)
                {
                    foreach (var audit in audits)
                    {
                        if (audit.Operation == "Deleted")
                        {
                            var nonTripV = new List<NonTripsView>();
                            if (audit.Ref_Table == "CIA")
                            {
                                nonTripV = _db.NonTripsViews.Where(t => t.CIA_Id == audit.RecordID).ToList();
                            }
                            else if (audit.Ref_Table == "ExpensesReport")
                            {
                                nonTripV = _db.NonTripsViews.Where(t => t.ExpenseReportId == audit.RecordID).ToList();
                            }
                            if (nonTripV.Count > 0)
                            {
                                _db.NonTripsViews.RemoveRange(nonTripV);
                                _db.SaveChanges();
                            }
                        }
                        else if (audit.Operation == "Insert" || audit.Operation == "Update")
                        {
                            if (audit.Operation == "Update")
                            {
                                var nonTripV = new List<NonTripsView>();
                                if (audit.Ref_Table == "CIA")
                                {
                                    nonTripV = _db.NonTripsViews.Where(t => t.CIA_Id == audit.RecordID).ToList();
                                }
                                else if (audit.Ref_Table == "ExpensesReport")
                                {
                                    nonTripV = _db.NonTripsViews.Where(t => t.ExpenseReportId == audit.RecordID).ToList();
                                }
                                if (nonTripV.Count > 0)
                                {
                                    _db.NonTripsViews.RemoveRange(nonTripV);
                                    _db.SaveChanges();
                                }
                            }

                            //var cia = _db.CashInAdvances.Where(a => a.Id == audit.RecordID).ToList();
                            //if (cia.Count > 0)
                            //{
                            //    _db.TripsViews.AddRange(SetNonTripsViewList(trip));
                            //    _db.SaveChanges();
                            //}
                        }
                    }

                    // Clear the related audit table records after syncing
                    _db.Audits.RemoveRange(audits);
                    _db.SaveChanges();
                }

                return View(_db.NonTripsViews.ToList());
            }
            ViewBag.ErrorMsg = "Not authenticated user.";
            return View("Error");
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

        public ActionResult Archive(string id)
        {
            var seY = id.Split('-'); // e.g. 2018-2019
            var startYear = seY[0];
            var endYear = seY[1];
            var cashInAdvances = _db.CashInAdvances.Where(c =>
                    ((c.RequestDate.Value.Year.ToString() == startYear && c.RequestDate.Value.Month > 6) ||
                    (c.RequestDate.Value.Year.ToString() == endYear && c.RequestDate.Value.Month < 7))
                    && c.TripID == null
                    ).ToList();
            var expenses = _db.ExpensesReports.Where(e =>
                ((e.SubmissionDate.Year.ToString() == startYear && e.SubmissionDate.Month > 6) ||
                 (e.SubmissionDate.Year.ToString() == endYear && e.SubmissionDate.Month < 7))
                 && e.TripID == null
                ).ToList();


            return View("Index", SetNonTripsViewList(cashInAdvances, expenses));
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
