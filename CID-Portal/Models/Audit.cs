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
    
    public partial class Audit
    {
        public int Id { get; set; }
        public System.DateTime Timestamp { get; set; }
        public string Ref_Table { get; set; }
        public int RecordID { get; set; }
        public string Operation { get; set; }
    }
}