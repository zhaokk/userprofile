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
    
    public partial class REFEREE
    {
        public REFEREE()
        {
            this.OFFERs = new HashSet<OFFER>();
            this.QUALIFICATIONS = new HashSet<QUALIFICATION>();
        }
    
        public int refID { get; set; }
        public string availability { get; set; }
        public Nullable<int> distTravel { get; set; }
        public string sport { get; set; }
        public Nullable<int> prefAge { get; set; }
        public Nullable<int> prefGrade { get; set; }
        public string ID { get; set; }
        
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<OFFER> OFFERs { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        public virtual ICollection<QUALIFICATION> QUALIFICATIONS { get; set; }
    }
}
