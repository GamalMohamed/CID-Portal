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
    
    public partial class VacationsLog
    {
        public int Id { get; set; }
        public Nullable<int> employeeId { get; set; }
        public Nullable<int> vacationBalanceBefore { get; set; }
        public Nullable<int> vacationBalanceAfter { get; set; }
        public Nullable<int> CompensationBalanceBefore { get; set; }
        public Nullable<int> CompensationBalanceAfter { get; set; }
        public string modifiedBy { get; set; }
        public Nullable<System.DateTime> modifiedOn { get; set; }
    
        public virtual Employee Employee { get; set; }
    }
}
