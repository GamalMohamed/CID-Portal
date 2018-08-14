using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace VacationsPortal.ViewModels
{
    public class TravelRequestViewModel
    {
        public int Id { get; set; }

        [DisplayName("Employee Name")]
        public string EmployeeName { get; set; }

        [DisplayName("Start date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("No. of days")]
        public int? NumOfDays { get; set; }

        [DisplayName("Working Days")]
        public int? WorkingDays { get; set; }

        [DisplayName("Modified On")]
        public DateTime? ModifiedOn { get; set; }

        [DisplayName("Is Cancelled")]
        public bool? IsCanceled { get; set; }
        
        public string Routes { get; set; }

        [DisplayName("TR Status")]
        public string TRStatus { get; set; }

        [DisplayName("Flight Status")]
        public string FlightStatus { get; set; }

        [DisplayName("Hotel Status")]
        public string HotelStatus { get; set; }

        [DisplayName("Visa Status")]
        public string VisaStatus { get; set; }

        [DisplayName("HR Letter Status")]
        public string HRLetterStatus { get; set; }

        public string FlightInvoiceStatus { get; set; }

        public string FlightInvoiceStatus2 { get; set; }

        public string ReissueInvoiceStatus { get; set; }

        public string ReissueInvoiceStatus2 { get; set; }

        public string ReissueInvoiceStatus3 { get; set; }

        public string HotelInvoiceStatus { get; set; }

        public string VisaInvoiceStatus { get; set; }

        public string FlightRefundInvoiceStatus { get; set; }
        
        [DisplayName("Flight Cost")]
        public double? FlightCost { get; set; }

        [DisplayName("Flight Cost Currency")]
        public string FlightCostCurrency { get; set; }

        [DisplayName("Flight Cost #2")]
        public double? FlightCost2 { get; set; }

        [DisplayName("Flight Cost #2 Currency")]
        public string FlightCost2Currency { get; set; }

        [DisplayName("Reissue Cost")]
        public double? ReissueCost { get; set; }

        [DisplayName("Reissue Cost 2")]
        public double? ReissueCost2 { get; set; }

        [DisplayName("Reissue Cost 3")]
        public double? ReissueCost3 { get; set; }

        [DisplayName("Flight Refund")]
        public double? FlightRefund { get; set; }

        [DisplayName("Hotel Cost")]
        public double? HotelCost { get; set; }

        [DisplayName("Hotel Cost Currency")]
        public string HotelCostCurrency { get; set; }

        [DisplayName("Requester Notes")]
        public string RequesterNotes { get; set; }

        [DisplayName("Operation Notes")]
        public string OperationsNotes { get; set; }

        [DisplayName("Trip Type")]
        public string TripType { get; set; }

    }
}