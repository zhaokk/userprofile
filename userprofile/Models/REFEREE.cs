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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public REFEREE()
        {
            this.OFFERs = new HashSet<OFFER>();
            this.OneOffAVAILABILITies = new HashSet<OneOffAVAILABILITY>();
            this.QUALIFICATIONS = new HashSet<QUALIFICATION>();
        }
    
        public int refID { get; set; }
        public Nullable<int> distTravel { get; set; }
        public string sport { get; set; }
        public Nullable<int> prefAge { get; set; }
        public Nullable<int> prefGrade { get; set; }
        public string ID { get; set; }
        public int maxGames { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OFFER> OFFERs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OneOffAVAILABILITY> OneOffAVAILABILITies { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        public virtual WEEKLYAVAILABILITY WEEKLYAVAILABILITY { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALIFICATION> QUALIFICATIONS { get; set; }
    }
}
