//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace userprofile.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class TIMEOFF
    {
        public int refID { get; set; }
        public Nullable<System.DateTime> startDate { get; set; }
        public Nullable<System.DateTime> endDate { get; set; }
        public Nullable<bool> allDay { get; set; }
        public Nullable<bool> whatType { get; set; }
        public string description { get; set; }
    
        public virtual REFEREE REFEREE { get; set; }
    }
}
