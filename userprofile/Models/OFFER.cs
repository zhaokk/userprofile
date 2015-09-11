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
    
    public partial class OFFER
    {
        public OFFER()
        {
            this.OFFERQUALs = new HashSet<OFFERQUAL>();
            this.TYPEs = new HashSet<TYPE>();
            this.REFEREEs = new HashSet<REFEREE>();
        }
    
        public int offerId { get; set; }
        public string sport { get; set; }
        public int matchId { get; set; }
        public int refId { get; set; }
        public int status { get; set; }
        public System.DateTime dateOfOffer { get; set; }
        public string declinedReason { get; set; }
        public int active { get; set; }
    
        public virtual MATCH MATCH { get; set; }
        public virtual ICollection<OFFERQUAL> OFFERQUALs { get; set; }
        public virtual REFEREE REFEREE { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        public virtual ICollection<TYPE> TYPEs { get; set; }
        public virtual ICollection<REFEREE> REFEREEs { get; set; }
    }
}
