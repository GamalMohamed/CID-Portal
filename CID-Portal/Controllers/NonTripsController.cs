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

        public List<NonTripsView> SetNonTripsViewList(List<CashInAdvance> cashInAdvances = null, List<ExpensesReport> expenses = null, bool expCia = false)
        {
            var nonTripsvm = new List<NonTripsView>();
            if (cashInAdvances != null)
            {
                foreach (var cashInAdvance in cashInAdvances)
                {
                    var nontripvm = new NonTripsView
                    {
                        EmployeeName = cashInAdvance.Employee.contact.FullName,
                        CIA_Id = cashInAdvance.Id,
                        CIA_Status = cashInAdvance.CashInAdvanceStatu.CashInAdvanceStatus,
                        CIA_Reason = cashInAdvance.Reason,
                        OperationsApprovalDate = cashInAdvance.OperationApprovalDate,
                        CurrencyName = cashInAdvance.Currency.CurrencyName,
                        CIA_Amount_InCurrency = cashInAdvance.Amount ?? 0,
                        CIA_ExchangeRate = cashInAdvance.ExchangeRate ?? 0
                    };
                    if (nontripvm.CIA_ExchangeRate != null)
                        nontripvm.CIA_Amount_InEGP = nontripvm.CIA_Amount_InCurrency *
                                                     (decimal)nontripvm.CIA_ExchangeRate;

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
            }

            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    if (expCia)
                    {
                        var nontripvm3 = new NonTripsView
                        {
                            EmployeeName = expense.Employee.contact.FullName,
                            ExpenseReportId = expense.ID,
                            Title = expense.Title,
                            SubmissionDate = expense.SubmissionDate,
                            ApprovalDate = expense.ApprovalDate,
                            ExpenseReportStatus = expense.ExpenseReportStatu.StatusName,
                            TotalAmountInEGP = expense.TotalAmountInUSD ?? 0,
                            CIAExpenseReport = expense.CashInAdvance ?? 0,
                            OperationsComment = expense.OperationsComment,
                            SettledAmount = expense.SettledAmount ?? 0,
                            SettlementDate = expense.SettlementDate
                        };
                        nontripvm3.AmountToEmployeeInEGP = (nontripvm3.TotalAmountInEGP -
                                                            nontripvm3.CIAExpenseReport) ?? 0;
                        nontripvm3.RemainingBalance = (decimal)nontripvm3.AmountToEmployeeInEGP -
                                                      nontripvm3.SettledAmount;

                        if (expense.CashInAdvances.Count > 0)
                        {
                            foreach (var cashInAdvance in expense.CashInAdvances)
                            {
                                var nontripvm4 = new NonTripsView
                                {
                                    EmployeeName = cashInAdvance.Employee.contact.FullName,
                                    CIA_Id = cashInAdvance.Id,
                                    CIA_Status = cashInAdvance.CashInAdvanceStatu.CashInAdvanceStatus,
                                    CIA_Reason = cashInAdvance.Reason,
                                    OperationsApprovalDate = cashInAdvance.OperationApprovalDate,
                                    CurrencyName = cashInAdvance.Currency.CurrencyName,
                                    CIA_Amount_InCurrency = cashInAdvance.Amount ?? 0,
                                    CIA_ExchangeRate = cashInAdvance.ExchangeRate ?? 0
                                };
                                if (nontripvm4.CIA_ExchangeRate != null)
                                    nontripvm4.CIA_Amount_InEGP = nontripvm4.CIA_Amount_InCurrency *
                                                                  (decimal)nontripvm4.CIA_ExchangeRate;

                                nonTripsvm.Add(nontripvm4); // Expense+CIA
                            }
                        }
                        else
                        {
                            nonTripsvm.Add(nontripvm3); // Expense only
                        }
                    }
                    else
                    {
                        // Default case (when filling)
                        if (expense.CashInAdvances.Count == 0)
                        {
                            var nontripvm3 = new NonTripsView
                            {
                                EmployeeName = expense.Employee.contact.FullName,
                                ExpenseReportId = expense.ID,
                                Title = expense.Title,
                                SubmissionDate = expense.SubmissionDate,
                                ApprovalDate = expense.ApprovalDate,
                                ExpenseReportStatus = expense.ExpenseReportStatu.StatusName,
                                TotalAmountInEGP = expense.TotalAmountInUSD ?? 0,
                                CIAExpenseReport = expense.CashInAdvance ?? 0,
                                OperationsComment = expense.OperationsComment,
                                SettledAmount = expense.SettledAmount ?? 0,
                                SettlementDate = expense.SettlementDate
                            };
                            nontripvm3.AmountToEmployeeInEGP = (nontripvm3.TotalAmountInEGP -
                                                                nontripvm3.CIAExpenseReport) ?? 0;
                            nontripvm3.RemainingBalance = (decimal)nontripvm3.AmountToEmployeeInEGP -
                                                          nontripvm3.SettledAmount;

                            nonTripsvm.Add(nontripvm3); // Expense only
                        }
                    }
                }
            }

            return nonTripsvm;
        }

        public List<NonTripsView_Archive> SetNonTripsViewListArchive(List<CashInAdvance> cashInAdvances = null, List<ExpensesReport> expenses = null, bool expCia = false)
        {
            var nonTripsvm = new List<NonTripsView_Archive>();
            if (cashInAdvances != null)
            {
                foreach (var cashInAdvance in cashInAdvances)
                {
                    var nontripvm = new NonTripsView_Archive
                    {
                        EmployeeName = cashInAdvance.Employee.contact.FullName,
                        CIA_Id = cashInAdvance.Id,
                        CIA_Status = cashInAdvance.CashInAdvanceStatu.CashInAdvanceStatus,
                        CIA_Reason = cashInAdvance.Reason,
                        OperationsApprovalDate = cashInAdvance.OperationApprovalDate,
                        CurrencyName = cashInAdvance.Currency.CurrencyName,
                        CIA_Amount_InCurrency = cashInAdvance.Amount ?? 0,
                        CIA_ExchangeRate = cashInAdvance.ExchangeRate ?? 0
                    };
                    if (nontripvm.CIA_ExchangeRate != null)
                        nontripvm.CIA_Amount_InEGP = nontripvm.CIA_Amount_InCurrency *
                                                     (decimal)nontripvm.CIA_ExchangeRate;

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
            }

            if (expenses != null)
            {
                foreach (var expense in expenses)
                {
                    if (expCia)
                    {
                        var nontripvm3 = new NonTripsView_Archive
                        {
                            EmployeeName = expense.Employee.contact.FullName,
                            ExpenseReportId = expense.ID,
                            Title = expense.Title,
                            SubmissionDate = expense.SubmissionDate,
                            ApprovalDate = expense.ApprovalDate,
                            ExpenseReportStatus = expense.ExpenseReportStatu.StatusName,
                            TotalAmountInEGP = expense.TotalAmountInUSD ?? 0,
                            CIAExpenseReport = expense.CashInAdvance ?? 0,
                            OperationsComment = expense.OperationsComment,
                            SettledAmount = expense.SettledAmount ?? 0,
                            SettlementDate = expense.SettlementDate
                        };
                        nontripvm3.AmountToEmployeeInEGP = (nontripvm3.TotalAmountInEGP -
                                                            nontripvm3.CIAExpenseReport) ?? 0;
                        nontripvm3.RemainingBalance = (decimal)nontripvm3.AmountToEmployeeInEGP -
                                                      nontripvm3.SettledAmount;

                        if (expense.CashInAdvances.Count > 0)
                        {
                            foreach (var cashInAdvance in expense.CashInAdvances)
                            {
                                var nontripvm4 = new NonTripsView_Archive
                                {
                                    EmployeeName = cashInAdvance.Employee.contact.FullName,
                                    CIA_Id = cashInAdvance.Id,
                                    CIA_Status = cashInAdvance.CashInAdvanceStatu.CashInAdvanceStatus,
                                    CIA_Reason = cashInAdvance.Reason,
                                    OperationsApprovalDate = cashInAdvance.OperationApprovalDate,
                                    CurrencyName = cashInAdvance.Currency.CurrencyName,
                                    CIA_Amount_InCurrency = cashInAdvance.Amount ?? 0,
                                    CIA_ExchangeRate = cashInAdvance.ExchangeRate ?? 0
                                };
                                if (nontripvm4.CIA_ExchangeRate != null)
                                    nontripvm4.CIA_Amount_InEGP = nontripvm4.CIA_Amount_InCurrency *
                                                                  (decimal)nontripvm4.CIA_ExchangeRate;

                                nonTripsvm.Add(nontripvm4); // Expense+CIA
                            }
                        }
                        else
                        {
                            nonTripsvm.Add(nontripvm3); // Expense only
                        }
                    }
                    else
                    {
                        // Default case (when filling)
                        if (expense.CashInAdvances.Count == 0)
                        {
                            var nontripvm3 = new NonTripsView_Archive
                            {
                                EmployeeName = expense.Employee.contact.FullName,
                                ExpenseReportId = expense.ID,
                                Title = expense.Title,
                                SubmissionDate = expense.SubmissionDate,
                                ApprovalDate = expense.ApprovalDate,
                                ExpenseReportStatus = expense.ExpenseReportStatu.StatusName,
                                TotalAmountInEGP = expense.TotalAmountInUSD ?? 0,
                                CIAExpenseReport = expense.CashInAdvance ?? 0,
                                OperationsComment = expense.OperationsComment,
                                SettledAmount = expense.SettledAmount ?? 0,
                                SettlementDate = expense.SettlementDate
                            };
                            nontripvm3.AmountToEmployeeInEGP = (nontripvm3.TotalAmountInEGP -
                                                                nontripvm3.CIAExpenseReport) ?? 0;
                            nontripvm3.RemainingBalance = (decimal)nontripvm3.AmountToEmployeeInEGP -
                                                          nontripvm3.SettledAmount;

                            nonTripsvm.Add(nontripvm3); // Expense only
                        }
                    }
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

            var nonTripViews = SetNonTripsViewList(cashInAdvances, expenses);

            _db.NonTripsViews.AddRange(nonTripViews);
            _db.SaveChanges();

            return RedirectToAction("Index", "NonTrips");
        }


        //public ActionResult FillNonTripsViewArchive()
        //{
        //    var id = "2013-2014";
        //    var seY = id.Split('-'); // e.g. 2018-2019
        //    var startYear = seY[0];
        //    var endYear = seY[1];
        //    var cashInAdvances = _db.CashInAdvances.Where(c =>
        //            ((c.RequestDate.Value.Year.ToString() == startYear && c.RequestDate.Value.Month > 6) ||
        //            (c.RequestDate.Value.Year.ToString() == endYear && c.RequestDate.Value.Month < 7))
        //            && c.TripID == null
        //            ).ToList();
        //    var expenses = _db.ExpensesReports.Where(e =>
        //        ((e.SubmissionDate.Year.ToString() == startYear && e.SubmissionDate.Month > 6) ||
        //         (e.SubmissionDate.Year.ToString() == endYear && e.SubmissionDate.Month < 7))
        //         && e.TripID == null
        //        ).ToList();

        //    var nonTripViews = SetNonTripsViewListArchive(cashInAdvances,expenses);

        //    _db.NonTripsView_Archive.AddRange(nonTripViews);
        //    _db.SaveChanges();

        //    return RedirectToAction("Index", "NonTrips");
        //}

        // GET: NonTrips
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                var audits = _db.Audits.Where(a => a.Ref_Table == "CIA" ||
                                              a.Ref_Table == "ExpensesReport").ToList();
                var auditsClone = new List<Audit>(audits);
                if (audits.Count > 0)
                {
                    foreach (var audit in audits)
                    {
                        if (audit.Operation == "Deleted")
                        {
                            if (audit.Ref_Table == "CIA")
                            {
                                var nontripVcia = _db.NonTripsViews.Where(t => t.CIA_Id == audit.RecordID).ToList();
                                if (nontripVcia.Count > 0)
                                {
                                    foreach (var nonTripsView in nontripVcia)
                                    {
                                        // If related expenses co-exist, just clear them
                                        if (nonTripsView.ExpenseReportId != null)
                                        {
                                            nonTripsView.CIA_Id = null;
                                            nonTripsView.CIA_Status = "";
                                            nonTripsView.CIA_Amount_InCurrency = null;
                                            nonTripsView.CurrencyName = "";
                                            nonTripsView.CIA_ExchangeRate = null;
                                            nonTripsView.CIA_Amount_InEGP = null;
                                            nonTripsView.CIA_Reason = "";
                                            nonTripsView.OperationsApprovalDate = null;
                                        }
                                        else
                                        {
                                            // No related expenses existing, so just simply delete the record
                                            _db.NonTripsViews.Remove(nonTripsView);
                                        }
                                        _db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    auditsClone.Remove(audit);
                                }
                            }
                            else if (audit.Ref_Table == "ExpensesReport")
                            {
                                var nontripVexp = _db.NonTripsViews.Where(t => t.ExpenseReportId == audit.RecordID).ToList();
                                if (nontripVexp.Count > 0)
                                {
                                    foreach (var nonTripsView in nontripVexp)
                                    {
                                        // If related CIA co-exist, just clear them
                                        if (nonTripsView.CIA_Id != null)
                                        {
                                            nonTripsView.ExpenseReportId = null;
                                            nonTripsView.SubmissionDate = new DateTime(0001, 1, 1); // set it to the default value
                                            nonTripsView.ApprovalDate = null;
                                            nonTripsView.ExpenseReportStatus = "";
                                            nonTripsView.TotalAmountInEGP = null;
                                            nonTripsView.CIAExpenseReport = null;
                                            nonTripsView.AmountToEmployeeInEGP = null;
                                            nonTripsView.SettledAmount = null;
                                            nonTripsView.SettlementDate = null;
                                            nonTripsView.RemainingBalance = null;
                                            nonTripsView.OperationsComment = "";
                                        }
                                        else
                                        {
                                            // No related cia existing, so just simply delete the record
                                            _db.NonTripsViews.Remove(nonTripsView);
                                        }
                                        _db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    auditsClone.Remove(audit);
                                }
                            }
                        }
                        else if (audit.Operation == "Insert" || audit.Operation == "Update")
                        {
                            if (audit.Ref_Table == "CIA")
                            {
                                var cia = _db.CashInAdvances.Find(audit.RecordID);
                                if (cia?.TripID == null)
                                {
                                    if (audit.Operation == "Update")
                                    {
                                        // Update CIA 
                                        var nontripV = _db.NonTripsViews.Where(t => t.CIA_Id == cia.Id).ToList();
                                        _db.NonTripsViews.RemoveRange(nontripV);
                                        _db.SaveChanges();

                                        //Insert new CIA
                                        _db.NonTripsViews.AddRange(SetNonTripsViewList(new List<CashInAdvance>() { cia }));
                                        _db.SaveChanges();
                                    }
                                    else
                                    {
                                        // Insert case 1: cia with existing Expenses
                                        if (cia?.ExpensesReports.Count > 0)
                                        {
                                            // Remove first all existing expenses records, then insert cia normally
                                            var expsIds = cia.ExpensesReports.Select(exp => exp.ID).ToList();
                                            foreach (var expsId in expsIds)
                                            {
                                                var nontripsVExp =
                                                    _db.NonTripsViews.Where(t => t.ExpenseReportId == expsId).ToList();
                                                _db.NonTripsViews.RemoveRange(nontripsVExp);
                                                _db.SaveChanges();
                                            }
                                        }
                                        // Insert case 2: cia without existing expenses
                                        _db.NonTripsViews.AddRange(SetNonTripsViewList(new List<CashInAdvance>() { cia }));
                                        _db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    auditsClone.Remove(audit);
                                }
                            }
                            else if (audit.Ref_Table == "ExpensesReport")
                            {
                                var exp = _db.ExpensesReports.Find(audit.RecordID);
                                if (exp != null && exp.TripID == null)
                                {
                                    if (audit.Operation == "Update")
                                    {
                                        // Update Expense 
                                        var nontripV =
                                            _db.NonTripsViews.Where(t => t.ExpenseReportId == exp.ID).ToList();
                                        _db.NonTripsViews.RemoveRange(nontripV);
                                        _db.SaveChanges();

                                        // Insert it again
                                        _db.NonTripsViews.AddRange(SetNonTripsViewList(null, new List<ExpensesReport> { exp }, true));
                                        _db.SaveChanges();
                                    }
                                    else
                                    {
                                        // Insert case 1: Expense with existing CIA
                                        if (exp.CashInAdvances.Count > 0)
                                        {
                                            // Remove first all existing expenses records, then insert cia normally
                                            var ciaIds = exp.CashInAdvances.Select(e => e.Id).ToList();
                                            foreach (var ciaId in ciaIds)
                                            {
                                                var nontripsVcia =
                                                    _db.NonTripsViews.Where(t => t.CIA_Id == ciaId).ToList();
                                                _db.NonTripsViews.RemoveRange(nontripsVcia);
                                                _db.SaveChanges();
                                            }
                                        }

                                        // Inserting Expense without existing CIA
                                        _db.NonTripsViews.AddRange(SetNonTripsViewList(null, new List<ExpensesReport> { exp }));
                                        _db.SaveChanges();
                                    }
                                }
                                else
                                {
                                    auditsClone.Remove(audit);
                                }
                            }
                        }
                    }

                    // Clear the related audit table records after syncing
                    _db.Audits.RemoveRange(auditsClone);
                    _db.SaveChanges();
                }

                return View(_db.NonTripsViews.ToList()
                            .OrderByDescending(t => t.ExpenseReportId)
                            .ThenByDescending(t => t.CIA_Id));
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

            var nontripsView = _db.NonTripsView_Archive.Where(e =>
                (e.SubmissionDate.Year.ToString() == startYear && e.SubmissionDate.Month > 6) ||
                (e.SubmissionDate.Year.ToString() == endYear && e.SubmissionDate.Month < 7) ||
                (e.OperationsApprovalDate.Value.Year.ToString() == startYear && e.OperationsApprovalDate.Value.Month > 5) ||
                (e.OperationsApprovalDate.Value.Year.ToString() == endYear && e.OperationsApprovalDate.Value.Month < 8))
                .ToList();

            return View(nontripsView);
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
