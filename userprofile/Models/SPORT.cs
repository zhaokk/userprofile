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
    
    public partial class SPORT
    {
        public SPORT()
        {
            this.OFFERs = new HashSet<OFFER>();
            this.QUALIFICATIONS = new HashSet<QUALIFICATION>();
            this.REFEREEs = new HashSet<REFEREE>();
            this.TOURNAMENTs = new HashSet<TOURNAMENT>();
        }
    
        public string name { get; set; }
    
        public virtual ICollection<OFFER> OFFERs { get; set; }
        public virtual ICollection<QUALIFICATION> QUALIFICATIONS { get; set; }
        public virtual ICollection<REFEREE> REFEREEs { get; set; }
        public virtual ICollection<TOURNAMENT> TOURNAMENTs { get; set; }
    }
}