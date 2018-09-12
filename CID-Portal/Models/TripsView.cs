//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VacationsPortal.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TripsView
    {
        public int Id { get; set; }
        public int TripID { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public string Country { get; set; }
        public Nullable<int> CIA_Id { get; set; }
        public string CIA_Status { get; set; }
        public Nullable<decimal> CIA_Amount_InCurrency { get; set; }
        public string CurrencyName { get; set; }
        public Nullable<double> CIA_ExchangeRate { get; set; }
        public Nullable<decimal> CIA_Amount_InEGP { get; set; }
        public string CIA_Reason { get; set; }
        public Nullable<System.DateTime> OperationsApprovalDate { get; set; }
        public Nullable<int> ExpenseReportId { get; set; }
        public System.DateTime SubmissionDate { get; set; }
        public Nullable<System.DateTime> ApprovalDate { get; set; }
        public string ExpenseReportStatus { get; set; }
        public Nullable<double> TotalAmountInEGP { get; set; }
        public Nullable<double> CIAExpenseReport { get; set; }
        public Nullable<double> AmountToEmployeeInEGP { get; set; }
        public Nullable<decimal> SettledAmount { get; set; }
        public Nullable<System.DateTime> SettlementDate { get; set; }
        public Nullable<decimal> RemainingBalance { get; set; }
        public string OperationsComment { get; set; }
    }
}
