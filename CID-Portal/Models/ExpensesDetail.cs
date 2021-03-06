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
    
    public partial class ExpensesDetail
    {
        public int ID { get; set; }
        public Nullable<System.DateTime> ExpensesDate { get; set; }
        public Nullable<int> ExpenseSubTypeID { get; set; }
        public Nullable<double> AmountInCurrency { get; set; }
        public int Currency { get; set; }
        public Nullable<double> ExchangeRate { get; set; }
        public Nullable<double> AmountInUSD { get; set; }
        public Nullable<bool> HasRecipt { get; set; }
        public System.DateTime SubmissionDate { get; set; }
        public Nullable<int> ExpenseReportID { get; set; }
        public int ExpensesCategoryID { get; set; }
        public Nullable<int> SubmissionSourceID { get; set; }
        public Nullable<double> USDExchange { get; set; }
        public Nullable<int> GeoID { get; set; }
        public string IOType { get; set; }
        public Nullable<bool> Valid { get; set; }
        public Nullable<bool> Validated { get; set; }
        public Nullable<int> RejectionReasonID { get; set; }
        public string RejectionMessage { get; set; }
        public Nullable<int> RouteId { get; set; }
    
        public virtual Country Country { get; set; }
        public virtual Currency Currency1 { get; set; }
        public virtual ExpenseCategory ExpenseCategory { get; set; }
        public virtual ExpensesReport ExpensesReport { get; set; }
        public virtual ExpensesSubCategoryType ExpensesSubCategoryType { get; set; }
        public virtual Route Route { get; set; }
    }
}
