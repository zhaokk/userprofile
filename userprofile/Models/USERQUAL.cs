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
    
    public partial class USERQUAL
    {
        [Display(Name = "Qualification Id")]
        public int qualificationId { get; set; }
        [Display(Name = "Referee Id")]
        public int refId { get; set; }
        [Display(Name = "Qualification Level")]
        public int qualLevel { get; set; }
    
        public virtual QUALIFICATION QUALIFICATION { get; set; }
        public virtual REFEREE REFEREE { get; set; }
    }
}
