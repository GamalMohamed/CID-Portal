using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace VacationsPortal.ViewModels
{
    public class TripViewModel
    {
        [DisplayName("Trip ID")]
        public int Id { get; set; }

        [DisplayName("Employee name")]
        public string EmployeeName { get; set; }

        [DisplayName("Start date")]
        public DateTime? StartDate { get; set; }

        [DisplayName("End date")]
        public DateTime? EndDate { get; set; }

        public string Country { get; set; }

        [DisplayName("CIA ID")]
        public int? CIA_Id { get; set; }

        [DisplayName("Status")]
        public string CIA_Status { get; set; }

        [DisplayName("Amount in Currency")]
        public decimal? CIA_Amount_InCurrency { get; set; }

        [DisplayName("Currency")]
        public string CurrencyName { get; set; }

        [DisplayName("Exchange rate")]
        public double? CIA_ExchangeRate { get; set; }

        [DisplayName("Amount in EGP")]
        public decimal? CIA_Amount_InEGP { get; set; }

        [DisplayName("Reason")]
        public string CIA_Reason { get; set; }

        [DisplayName("Operations Approval date")]
        public DateTime? OperationsApprovalDate { get; set; }

        [DisplayName("Expenses Report Id")]
        public int? ExpenseReportId { get; set; }

        [DisplayName("Submission date")]
        public DateTime SubmissionDate { get; set; }

        [DisplayName("Approval date")]
        public DateTime? ApprovalDate { get; set; }

        [DisplayName("Status")]
        public string ExpenseReportStatus { get; set; }

        [DisplayName("Total amount in EGP")]
        public double? TotalAmountInEGP { get; set; }

        [DisplayName("CIA Expense Report")]
        public double? CIAExpenseReport { get; set; }

        [DisplayName("Amount to Employee in EGP")]
        public double? AmountToEmployeeInEGP { get; set; }

        [DisplayName("Settled amount")]
        public decimal? SettledAmount { get; set; }

        [DisplayName("Settlement date")]
        public DateTime? SettlementDate { get; set; }

        [DisplayName("Remaining Balance")]
        public decimal? RemainingBalance { get; set; }

        [DisplayName("Operations comment")]
        public string OperationsComment { get; set; }
    
    }
}