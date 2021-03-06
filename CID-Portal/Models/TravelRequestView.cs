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
    
    public partial class TravelRequestView
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> NumOfDays { get; set; }
        public Nullable<int> WorkingDays { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<bool> IsCanceled { get; set; }
        public string Routes { get; set; }
        public string TRStatus { get; set; }
        public string FlightStatus { get; set; }
        public string HotelStatus { get; set; }
        public string VisaStatus { get; set; }
        public string HRLetterStatus { get; set; }
        public string FlightInvoiceStatus { get; set; }
        public string FlightInvoiceStatus2 { get; set; }
        public string ReissueInvoiceStatus { get; set; }
        public string ReissueInvoiceStatus2 { get; set; }
        public string ReissueInvoiceStatus3 { get; set; }
        public string HotelInvoiceStatus { get; set; }
        public string VisaInvoiceStatus { get; set; }
        public string FlightRefundInvoiceStatus { get; set; }
        public Nullable<double> FlightCost { get; set; }
        public string FlightCostCurrency { get; set; }
        public Nullable<double> FlightCost2 { get; set; }
        public string FlightCost2Currency { get; set; }
        public Nullable<double> ReissueCost { get; set; }
        public Nullable<double> ReissueCost2 { get; set; }
        public Nullable<double> ReissueCost3 { get; set; }
        public Nullable<double> FlightRefund { get; set; }
        public Nullable<double> HotelCost { get; set; }
        public string HotelCostCurrency { get; set; }
        public string RequesterNotes { get; set; }
        public string OperationsNotes { get; set; }
        public string TripType { get; set; }
    }
}
