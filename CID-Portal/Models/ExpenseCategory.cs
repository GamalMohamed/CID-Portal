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
    
    public partial class ExpenseCategory
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExpenseCategory()
        {
            this.ExpensesDetails = new HashSet<ExpensesDetail>();
        }
    
        public int ID { get; set; }
        public string ExpenseName { get; set; }
        public Nullable<int> Opex { get; set; }
        public Nullable<int> NonTripType { get; set; }
        public Nullable<bool> HasSubCategory { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExpensesDetail> ExpensesDetails { get; set; }
    }
}
