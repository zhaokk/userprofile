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
    
    public partial class LOCATION
    {
        public LOCATION()
        {
            this.MATCHes = new HashSet<MATCH>();
        }
    
        public int lID { get; set; }
        [Display(Name="location")]
        public string name { get; set; }
        public Nullable<double> price { get; set; }
        public string street { get; set; }
        public Nullable<int> snum { get; set; }
        public string city { get; set; }
        public int postcode { get; set; }
        public int phoneNum { get; set; }
        public string state { get; set; }
    
        public virtual ICollection<MATCH> MATCHes { get; set; }
    }
}
