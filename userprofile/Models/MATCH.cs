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
    
    public partial class MATCH
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public MATCH()
        {
            this.INFRACTIONS = new HashSet<INFRACTION>();
            this.OFFERs = new HashSet<OFFER>();
        }
    
        public int matchId { get; set; }
        public System.DateTime matchDate { get; set; }
        public Nullable<int> locationId { get; set; }
        public int teamAId { get; set; }
        public int teamBId { get; set; }
        public Nullable<int> teamAScore { get; set; }
        public Nullable<int> teamBScore { get; set; }
        public Nullable<int> status { get; set; }
        public int tournamentId { get; set; }
        public int matchLength { get; set; }
        public int halfTimeDuration { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<INFRACTION> INFRACTIONS { get; set; }
        public virtual LOCATION LOCATION { get; set; }
        public virtual TEAM TEAM { get; set; }
        public virtual TEAM TEAM1 { get; set; }
        public virtual TOURNAMENT TOURNAMENT { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFER> OFFERs { get; set; }
    }
}
