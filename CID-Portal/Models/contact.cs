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
    
    public partial class contact
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PassportNumber { get; set; }
        public string BasedOut { get; set; }
        public string Nationality { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public Nullable<System.DateTime> Birthday { get; set; }
        public string Personal_Mail { get; set; }
        public Nullable<int> BaseCity { get; set; }
        public string FullName { get; set; }
    
        public virtual Employee Employee { get; set; }
        public virtual City City { get; set; }
    }
}
