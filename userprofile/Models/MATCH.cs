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
    using System.ComponentModel.DataAnnotations;
    
    public partial class MATCH
    {
        public MATCH()
        {
            this.INFRACTIONS = new HashSet<INFRACTION>();
            this.MATCHQUALs = new HashSet<MATCHQUAL>();
            this.OFFERs = new HashSet<OFFER>();
            this.SCORES = new HashSet<SCORE>();
        }
    
        public int mID { get; set; }

        public Nullable<System.DateTime> matchDate { get; set; }
        public Nullable<int> location { get; set; }
        public Nullable<int> teamaID { get; set; }
        public Nullable<int> teambID { get; set; }
        public Nullable<bool> winnerID { get; set; }
        public Nullable<int> tournament { get; set; }
    
        public virtual ICollection<INFRACTION> INFRACTIONS { get; set; }
        public virtual LOCATION LOCATION1 { get; set; }
        public virtual ICollection<MATCHQUAL> MATCHQUALs { get; set; }
        public virtual TEAM TEAM { get; set; }
        public virtual TEAM TEAM1 { get; set; }
        public virtual TOURNAMENT TOURNAMENT1 { get; set; }
        public virtual ICollection<OFFER> OFFERs { get; set; }
        public virtual ICollection<SCORE> SCORES { get; set; }
    }
}
