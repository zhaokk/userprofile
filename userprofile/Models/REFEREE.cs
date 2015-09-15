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
    
    public partial class REFEREE
    {
        public REFEREE()
        {
            this.OFFERs = new HashSet<OFFER>();
            this.OneOffAVAILABILITies = new HashSet<OneOffAVAILABILITY>();
            this.USERQUALs = new HashSet<USERQUAL>();
            this.OFFERs1 = new HashSet<OFFER>();
            this.LOCATIONs = new HashSet<LOCATION>();
        }

        [Display(Name = "referee Id")]
        public int refId { get; set; }
        [Display(Name = "Travel Distance")]
        public Nullable<int> distTravel { get; set; }
        [Display(Name = "Sport")]
        public string sport { get; set; }
        [Display(Name = "User Id")]
        public string userId { get; set; }
        [Display(Name = "Max Games")]
        public int maxGames { get; set; }
        [Display(Name = "Status")]
        public int status { get; set; }
        [Display(Name = "Rating")]
        public int rating { get; set; }
    
        public virtual AspNetUser AspNetUser { get; set; }
        public virtual ICollection<OFFER> OFFERs { get; set; }
        public virtual ICollection<OneOffAVAILABILITY> OneOffAVAILABILITies { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        public virtual ICollection<USERQUAL> USERQUALs { get; set; }
        public virtual WEEKLYAVAILABILITY WEEKLYAVAILABILITY { get; set; }
        public virtual ICollection<OFFER> OFFERs1 { get; set; }
        public virtual ICollection<LOCATION> LOCATIONs { get; set; }
    }
}
