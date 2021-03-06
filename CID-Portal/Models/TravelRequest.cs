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
    
    public partial class TravelRequest
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TravelRequest()
        {
            this.TRHotelInfoes = new HashSet<TRHotelInfo>();
            this.Trips = new HashSet<Trip>();
        }
    
        public int TRID { get; set; }
        public string RequesterNotes { get; set; }
        public System.DateTime RequestedOn { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<int> TRStatusID { get; set; }
        public Nullable<int> FlightStatus { get; set; }
        public Nullable<double> TotalFlightCost { get; set; }
        public Nullable<int> HotelStatus { get; set; }
        public Nullable<int> VisaStatus { get; set; }
        public Nullable<int> HRLetterStatus { get; set; }
        public Nullable<int> InvitationLetterStatus { get; set; }
        public string OperationsNotes { get; set; }
        public Nullable<System.DateTime> ChangeRequestDate { get; set; }
        public string ChangeRequestReason { get; set; }
        public string TravelRequestForm { get; set; }
        public Nullable<System.DateTime> UpdatedOn { get; set; }
        public string UpdatedBy { get; set; }
        public Nullable<int> ExcelStatusId { get; set; }
        public Nullable<double> TotalReissueCost { get; set; }
        public Nullable<int> Rating { get; set; }
        public string RatingComment { get; set; }
        public Nullable<int> TotalFlightCostCurrencyID { get; set; }
        public Nullable<double> TotalHotelCost { get; set; }
        public Nullable<int> TotalHotelCostCurrencyID { get; set; }
        public Nullable<double> TotalReissueCost2 { get; set; }
        public Nullable<double> TotalReissueCost3 { get; set; }
        public Nullable<double> FlightRefundCost { get; set; }
        public Nullable<double> ForcastedFlightsCost { get; set; }
        public Nullable<double> ForcastedAccommodationCost { get; set; }
        public Nullable<double> ForcastedExpenses { get; set; }
        public Nullable<double> ForcastedTotal { get; set; }
        public Nullable<double> TotalFlightCost2 { get; set; }
        public Nullable<int> TotalFlightCostCurrencyID2 { get; set; }
        public Nullable<int> FlightInvoiceStatus { get; set; }
        public Nullable<int> FlightInvoiceStatus2 { get; set; }
        public Nullable<int> TotalReissueStatus { get; set; }
        public Nullable<int> TotalReissueStatus2 { get; set; }
        public Nullable<int> TotalReissueStatus3 { get; set; }
        public Nullable<int> VisaInvoiceStatus { get; set; }
        public Nullable<int> HoteInvoiceStatus { get; set; }
        public Nullable<int> PoStatus { get; set; }
        public Nullable<int> FlightRefundInvoiceStatus { get; set; }
    
        public virtual Currency Currency { get; set; }
        public virtual Currency Currency1 { get; set; }
        public virtual Currency Currency2 { get; set; }
        public virtual TRItemsStatu TRItemsStatu { get; set; }
        public virtual TRItemsStatu TRItemsStatu1 { get; set; }
        public virtual TRItemsStatu TRItemsStatu2 { get; set; }
        public virtual TRItemsStatu TRItemsStatu3 { get; set; }
        public virtual TRItemsStatu TRItemsStatu4 { get; set; }
        public virtual TRItemsStatu TRItemsStatu5 { get; set; }
        public virtual TravelRequestStatu TravelRequestStatu { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TRHotelInfo> TRHotelInfoes { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Trip> Trips { get; set; }
    }
}
