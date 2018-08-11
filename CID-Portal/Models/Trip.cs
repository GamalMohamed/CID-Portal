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
    
    public partial class Trip
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Trip()
        {
            this.CashInAdvances = new HashSet<CashInAdvance>();
            this.ExpensesReports = new HashSet<ExpensesReport>();
            this.Routes = new HashSet<Route>();
        }
    
        public int Id { get; set; }
        public Nullable<int> EmployeeId { get; set; }
        public Nullable<System.DateTime> StartDate { get; set; }
        public Nullable<System.DateTime> EndDate { get; set; }
        public Nullable<int> NumberofDays { get; set; }
        public Nullable<int> NumberofWorkingDays { get; set; }
        public Nullable<int> Status { get; set; }
        public string ModifiedBy { get; set; }
        public Nullable<System.DateTime> ModifiedOn { get; set; }
        public Nullable<bool> Business { get; set; }
        public Nullable<bool> IsCanceled { get; set; }
        public Nullable<int> TRID { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CashInAdvance> CashInAdvances { get; set; }
        public virtual Employee Employee { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpensesReport> ExpensesReports { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Route> Routes { get; set; }
        public virtual TravelRequest TravelRequest { get; set; }
    }
}