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
    
    public partial class TOURNAMENT
    {
        public TOURNAMENT()
        {
            this.MATCHes = new HashSet<MATCH>();
            this.TEAMs = new HashSet<TEAM>();
        }
    
        public int tID { get; set; }
        public string sport { get; set; }
    
        public virtual ICollection<MATCH> MATCHes { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        public virtual ICollection<TEAM> TEAMs { get; set; }
    }
}
