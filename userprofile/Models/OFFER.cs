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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OFFER()
        {
            this.TYPEs = new HashSet<TYPE>();
            this.QUALIFICATIONS = new HashSet<QUALIFICATION>();
        }
    
        public int offerID { get; set; }
        public string sport { get; set; }
        public int mid { get; set; }
        public Nullable<int> refID { get; set; }
        public int status { get; set; }
        public Nullable<System.DateTime> dateOfOffer { get; set; }
    
        public virtual MATCH MATCH { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TYPE> TYPEs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QUALIFICATION> QUALIFICATIONS { get; set; }
        public virtual REFEREE REFEREE { get; set; }
    }
}
