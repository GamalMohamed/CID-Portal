using System;
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
                        var tripvm2 = new TripsView
                        {
                            TripID = tripvm.TripID,
                            EmployeeName = tripvm.EmployeeName,
                            StartDate = tripvm.StartDate,
                            EndDate = tripvm.EndDate,
                            Country = tripvm.Country,
                            CIA_Id = cia.Id,
                            CIA_Status = cia.CashInAdvanceStatu.CashInAdvanceStatus,
                            CurrencyName = cia.Currency.CurrencyName,
                            CIA_Amount_InCurrency = cia.Amount ?? 0,
                            CIA_ExchangeRate = cia.ExchangeRate ?? 0,
                            CIA_Reason = cia.Reason,
                            OperationsApprovalDate = cia.OperationApprovalDate
                        };
                        if (tripvm2.CIA_ExchangeRate != null)
                            tripvm2.CIA_Amount_InEGP = tripvm2.CIA_Amount_InCurrency *
                                                       (decimal)tripvm2.CIA_ExchangeRate;


                        if (cia.ExpensesReports.Count > 0)
                        {
                            foreach (var ciaExpensesReport in cia.ExpensesReports)
                            {
                                var tripvm3 = new TripsView
                                {
                                    TripID = tripvm2.TripID,
                                    EmployeeName = tripvm2.EmployeeName,
                                    StartDate = tripvm2.StartDate,
                                    EndDate = tripvm2.EndDate,
                                    Country = tripvm2.Country,
                                    CIA_Id = tripvm2.CIA_Id,
                                    CIA_Status = tripvm2.CIA_Status,
                                    CurrencyName = tripvm2.CurrencyName,
                                    CIA_Amount_InCurrency = tripvm2.CIA_Amount_InCurrency ?? 0,
                                    CIA_ExchangeRate = tripvm2.CIA_ExchangeRate ?? 0,
                                    CIA_Reason = tripvm2.CIA_Reason,
                                    OperationsApprovalDate = tripvm2.OperationsApprovalDate,
                                    ExpenseReportId = ciaExpensesReport.ID,
                                    SubmissionDate = ciaExpensesReport.SubmissionDate,
                                    ApprovalDate = ciaExpensesReport.ApprovalDate,
                                    ExpenseReportStatus = ciaExpensesReport.ExpenseReportStatu.StatusName,
                                    TotalAmountInEGP = ciaExpensesReport.TotalAmountInUSD ?? 0,
                                    CIAExpenseReport = ciaExpensesReport.CashInAdvance ?? 0,
                                    SettledAmount = ciaExpensesReport.SettledAmount ?? 0,
                                    SettlementDate = ciaExpensesReport.SettlementDate,
                                    OperationsComment = ciaExpensesReport.OperationsComment
                                };
                                if (tripvm3.CIA_ExchangeRate != null)
                                    tripvm3.CIA_Amount_InEGP = tripvm3.CIA_Amount_InCurrency *
                                                               (decimal)tripvm3.CIA_ExchangeRate;
                                tripvm3.AmountToEmployeeInEGP = (tripvm3.TotalAmountInEGP - tripvm3.CIAExpenseReport) ?? 0;
                                tripvm3.RemainingBalance = Math.Abs((decimal)tripvm3.AmountToEmployeeInEGP) - tripvm3.SettledAmount;

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
                        var tripvm2 = new TripsView_Archive
                        {
                            TripID = tripvm.TripID,
                            EmployeeName = tripvm.EmployeeName,
                            StartDate = tripvm.StartDate,
                            EndDate = tripvm.EndDate,
                            Country = tripvm.Country,
                            CIA_Id = cia.Id,
                            CIA_Status = cia.CashInAdvanceStatu.CashInAdvanceStatus,
                            CurrencyName = cia.Currency.CurrencyName,
                            CIA_Amount_InCurrency = cia.Amount ?? 0,
                            CIA_ExchangeRate = cia.ExchangeRate ?? 0,
                            CIA_Reason = cia.Reason,
                            OperationsApprovalDate = cia.OperationApprovalDate
                        };
                        if (tripvm2.CIA_ExchangeRate != null)
                            tripvm2.CIA_Amount_InEGP = tripvm2.CIA_Amount_InCurrency *
                                                       (decimal)tripvm2.CIA_ExchangeRate;


                        if (cia.ExpensesReports.Count > 0)
                        {
                            foreach (var ciaExpensesReport in cia.ExpensesReports)
                            {
                                var tripvm3 = new TripsView_Archive
                                {
                                    TripID = tripvm2.TripID,
                                    EmployeeName = tripvm2.EmployeeName,
                                    StartDate = tripvm2.StartDate,
                                    EndDate = tripvm2.EndDate,
                                    Country = tripvm2.Country,
                                    CIA_Id = tripvm2.CIA_Id,
                                    CIA_Status = tripvm2.CIA_Status,
                                    CurrencyName = tripvm2.CurrencyName,
                                    CIA_Amount_InCurrency = tripvm2.CIA_Amount_InCurrency ?? 0,
                                    CIA_ExchangeRate = tripvm2.CIA_ExchangeRate ?? 0,
                                    CIA_Reason = tripvm2.CIA_Reason,
                                    OperationsApprovalDate = tripvm2.OperationsApprovalDate,
                                    ExpenseReportId = ciaExpensesReport.ID,
                                    SubmissionDate = ciaExpensesReport.SubmissionDate,
                                    ApprovalDate = ciaExpensesReport.ApprovalDate,
                                    ExpenseReportStatus = ciaExpensesReport.ExpenseReportStatu.StatusName,
                                    TotalAmountInEGP = ciaExpensesReport.TotalAmountInUSD ?? 0,
                                    CIAExpenseReport = ciaExpensesReport.CashInAdvance ?? 0,
                                    SettledAmount = ciaExpensesReport.SettledAmount ?? 0,
                                    SettlementDate = ciaExpensesReport.SettlementDate,
                                    OperationsComment = ciaExpensesReport.OperationsComment
                                };
                                if (tripvm3.CIA_ExchangeRate != null)
                                    tripvm3.CIA_Amount_InEGP = tripvm3.CIA_Amount_InCurrency *
                                                               (decimal)tripvm3.CIA_ExchangeRate;
                                tripvm3.AmountToEmployeeInEGP = (tripvm3.TotalAmountInEGP - tripvm3.CIAExpenseReport) ?? 0;
                                tripvm3.RemainingBalance = Math.Abs((decimal)tripvm3.AmountToEmployeeInEGP) - tripvm3.SettledAmount;

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

        // CAUTION: THESE ROUTES ARE FOR DEVELOPMENT PURPOSES ONLY!!
        public ActionResult FillTripsView()
        {
            List<Trip> trips;
            if (DateTime.Now.Month > 6)
            {
                trips = _db.Trips.Where(t =>
                    (t.StartDate.Value.Year == DateTime.Now.Year - 1 && t.StartDate.Value.Month > 6) ||
                    (t.StartDate.Value.Year == DateTime.Now.Year)
                    ).Include(t => t.Employee.contact)
                     .Include(t => t.Routes)
                     .Include(t => t.CashInAdvances)
                     .Include(t => t.ExpensesReports)
                     .ToList();
            }
            else
            {
                trips = _db.Trips.Where(t =>
                    (t.StartDate.Value.Year == DateTime.Now.Year - 2 && t.StartDate.Value.Month > 6) ||
                    (t.StartDate.Value.Year == DateTime.Now.Year - 1) ||
                    (t.StartDate.Value.Year == DateTime.Now.Year)
                    ).Include(t => t.Employee.contact)
                     .Include(t => t.Routes)
                     .Include(t => t.CashInAdvances)
                     .Include(t => t.ExpensesReports)
                     .ToList();
            }

            var tripViews = SetTripsViewList(trips);
            _db.TripsViews.AddRange(tripViews);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        public ActionResult FillTripsViewArchive()
        {
            var s = "2013-2014";
            var seY = s.Split('-'); // e.g. 2018-2019
            var startYear = seY[0];
            var endYear = seY[1];
            var trips = _db.Trips.Where(t =>
                    (t.StartDate.Value.Year.ToString() == startYear && t.StartDate.Value.Month > 6) ||
                    (t.StartDate.Value.Year.ToString() == endYear && t.StartDate.Value.Month < 7)
                    ).ToList();

            var tripViews = SetTripsViewListArchive(trips.OrderByDescending(t => t.StartDate).ToList());
            _db.TripsView_Archive.AddRange(tripViews);
            _db.SaveChanges();

            return RedirectToAction("Index");
        }

        // GET: Trips
        public ActionResult Index()
        {
            if (IsAuthorized())
            {
                var audits = _db.Audits.Where(a => a.Ref_Table == "Trips" ||
                                              a.Ref_Table == "CIA" ||
                                              a.Ref_Table == "ExpensesReport" ||
                                              a.Ref_Table == "Route").ToList();
                var auditsClone = new List<Audit>(audits);
                if (audits.Count > 0)
                {
                    foreach (var audit in audits)
                    {
                        if (audit.Operation == "Deleted")
                        {
                            if (audit.Ref_Table == "Trips")
                            {
                                // Delete All related TripView records
                                var tripV = _db.TripsViews.Where(t => t.TripID == audit.RecordID).ToList();
                                if (tripV.Count > 0)
                                {
                                    _db.TripsViews.RemoveRange(tripV);
                                    _db.SaveChanges();
                                }
                            }
                            else if (audit.Ref_Table == "CIA")
                            {
                                // Get all tripViews records containing this CIA
                                var tripVcia = _db.TripsViews.Where(t => t.CIA_Id == audit.RecordID).ToList();
                                if (tripVcia.Count > 0)
                                {
                                    // Get the related trip for this CIA
                                    var trip = _db.Trips.Find(tripVcia[0].TripID);
                                    // Get All tripViews realted to this trip
                                    var tripsVAll = _db.TripsViews.Where(t => t.TripID == tripVcia[0].TripID).ToList();
                                    
                                    // Remove all tripViews of this trip
                                    _db.TripsViews.RemoveRange(tripsVAll);
                                    _db.SaveChanges();

                                    //Recreate the tripViews again
                                    _db.TripsViews.AddRange(SetTripsViewList(new List<Trip> { trip }));
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    auditsClone.Remove(audit); //Non-trip CIA
                                }
                            }
                            else if (audit.Ref_Table == "ExpensesReport")
                            {
                                // Delete the Expenses Fields in each related TripView
                                var tripVexp = _db.TripsViews.Where(t => t.ExpenseReportId == audit.RecordID).ToList();
                                if (tripVexp.Count > 0)
                                {
                                    var trip = _db.Trips.Find(tripVexp[0].TripID);
                                    var tripsVAll = _db.TripsViews.Where(t => t.TripID == tripVexp[0].TripID).ToList();
                                    _db.TripsViews.RemoveRange(tripsVAll);
                                    _db.SaveChanges();

                                    _db.TripsViews.AddRange(SetTripsViewList(new List<Trip> { trip }));
                                    _db.SaveChanges();
                                }
                                else
                                {
                                    auditsClone.Remove(audit); // Non-trip expense
                                }
                            }
                            else if (audit.Ref_Table == "Route")
                            {
                                // Same as in the insert/update!
                                if (audit.TripID != null)
                                {
                                    var trip = _db.Trips.Find(audit.TripID);
                                    if (trip != null)
                                    {
                                        // Update TripView
                                        var tripV = _db.TripsViews.Where(t => t.TripID == trip.Id).ToList();
                                        if (tripV.Count > 0)
                                        {
                                            foreach (var trv in tripV)
                                            {
                                                trv.Country = trip.Routes.ToList()[trip.Routes.Count - 2].Country.CountryName;
                                            }
                                        }
                                        _db.SaveChanges();

                                        // Sign!
                                        var aud = _db.Audits.Find(audit.Id);
                                        if (aud != null)
                                        {
                                            var remark = aud.Remark;
                                            aud.Remark = "T" + remark[1].ToString() + remark[2].ToString();
                                            _db.SaveChanges();
                                            if (aud.Remark != "TRV")
                                            {
                                                auditsClone.Remove(aud);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else if (audit.Operation == "Insert" || audit.Operation == "Update")
                        {
                            if (audit.Ref_Table == "Trips")
                            {
                                if (audit.Operation == "Update")
                                {
                                    if (audit.Remark[0] == 'X')
                                    {
                                        var trip = _db.Trips.Find(audit.RecordID);
                                        if (trip != null)
                                        {
                                            // Update TripView
                                            var tripV = _db.TripsViews.Where(t => t.TripID == audit.RecordID).ToList();
                                            if (tripV.Count > 0)
                                            {
                                                foreach (var trv in tripV)
                                                {
                                                    if (trip.StartDate != null)
                                                    {
                                                        trv.StartDate = trip.StartDate.Value;
                                                    }
                                                    if (trip.EndDate != null)
                                                    {
                                                        trv.EndDate = trip.EndDate.Value;
                                                    }
                                                }
                                            }
                                            _db.SaveChanges();

                                            // Sign!
                                            var aud = _db.Audits.Find(audit.Id);
                                            if (aud != null)
                                            {
                                                var remark = aud.Remark;
                                                aud.Remark = "T" + remark[1].ToString() + remark[2].ToString();
                                                _db.SaveChanges();
                                                if (aud.Remark != "TRV")
                                                {
                                                    auditsClone.Remove(aud);
                                                }
                                            }
                                        }
                                    }
                                }
                                else if (audit.Operation == "Insert")
                                {
                                    var trip = _db.Trips.Where(a => a.Id == audit.RecordID).ToList();
                                    // Single trip, but returning it as a list just to be accepted by the compiler for the setviewfunction
                                    if (trip.Count > 0)
                                    {
                                        _db.TripsViews.AddRange(SetTripsViewList(trip)); // Creating a totally new view!
                                        _db.SaveChanges();
                                    }
                                }
                            }
                            else if (audit.Ref_Table == "CIA")
                            {
                                var cia = _db.CashInAdvances.Find(audit.RecordID);
                                if (cia != null)
                                {
                                    if (cia.TripID != null)
                                    {
                                        // Delete Outdated tripView records
                                        var tripV = _db.TripsViews.Where(t => t.TripID == cia.TripID).ToList();
                                        _db.TripsViews.RemoveRange(tripV);
                                        _db.SaveChanges();

                                        // Re-create the tripView again
                                        _db.TripsViews.AddRange(SetTripsViewList(new List<Trip> { cia.Trip }));
                                        _db.SaveChanges();
                                    }
                                    else
                                    {
                                        auditsClone.Remove(audit); // Non-trip CIA
                                    }
                                }
                            }
                            else if (audit.Ref_Table == "ExpensesReport")
                            {
                                var exp = _db.ExpensesReports.Find(audit.RecordID);
                                if (exp != null)
                                {
                                    if (exp.TripID != null)
                                    {
                                        var tripV = _db.TripsViews.Where(t => t.TripID == exp.TripID).ToList();
                                        _db.TripsViews.RemoveRange(tripV);
                                        _db.SaveChanges();

                                        _db.TripsViews.AddRange(SetTripsViewList(new List<Trip>() { exp.Trip }));
                                        _db.SaveChanges();
                                    }
                                    else
                                    {
                                        auditsClone.Remove(audit); // Non-Trip expense
                                    }
                                }
                            }
                            else if (audit.Ref_Table == "Route")
                            {
                                // Each of the Trips, Travel Requests and Visas views has its own sign 
                                if (audit.Remark[0] == 'X')
                                {
                                    if (audit.TripID != null)
                                    {
                                        var trip = _db.Trips.Find(audit.TripID);
                                        if (trip != null)
                                        {
                                            // Update TripView
                                            var tripV = _db.TripsViews.Where(t => t.TripID == trip.Id).ToList();
                                            if (tripV.Count > 0)
                                            {
                                                foreach (var trv in tripV)
                                                {
                                                    trv.Country = trip.Routes.ToList()[trip.Routes.Count - 2].Country.CountryName;
                                                }
                                            }
                                            _db.SaveChanges();

                                            // Sign!
                                            var aud = _db.Audits.Find(audit.Id);
                                            if (aud != null)
                                            {
                                                var remark = aud.Remark;
                                                aud.Remark = "T" + remark[1].ToString() + remark[2].ToString();
                                                _db.SaveChanges();
                                                if (aud.Remark != "TRV")
                                                {
                                                    auditsClone.Remove(aud);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Clear the related audit table records after syncing
                    _db.Audits.RemoveRange(auditsClone);
                    _db.SaveChanges();
                }

                return View(_db.TripsViews.OrderByDescending(t => t.Id).ToList());
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
            if (id != "")
            {
                var seY = id.Split('-'); // e.g. 2018-2019
                if (seY.Length == 2 )
                {
                    var startYear = seY[0];
                    var endYear = seY[1];
                    var trips = _db.TripsView_Archive.Where(t =>
                            (t.StartDate.Value.Year.ToString() == startYear && t.StartDate.Value.Month > 6) ||
                            (t.StartDate.Value.Year.ToString() == endYear && t.StartDate.Value.Month < 7)
                            ).ToList();

                    return View(trips);
                }
            }
            return RedirectToAction("Index");
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
