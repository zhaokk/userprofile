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
    
    public partial class QUALIFICATION
    {
        public QUALIFICATION()
        {
            this.OFFERQUALs = new HashSet<OFFERQUAL>();
            this.USERQUALs = new HashSet<USERQUAL>();
        }

        [Display(Name = "Qualification Id")]
        [Required(ErrorMessage = "Qulification Id is required")]
        public int qualificationId { get; set; }
        [Display(Name = "Qualification")]
        [Required(ErrorMessage = "Qulification is required")]
        public string name { get; set; }
        [Display(Name = "Sport")]
        [Required(ErrorMessage = "Sport is required")]
        public string sport { get; set; }
        [Display(Name = "Description")]
        [Required(ErrorMessage = "Description is required")]
        public string description { get; set; }
        [Display(Name = "Qualification Level")]
        public int qualificationLevel { get; set; }
        [Display(Name = "Status")]
        public int status { get; set; }

        public int qlevelForEditView { get; set; }
    
        public virtual ICollection<OFFERQUAL> OFFERQUALs { get; set; }
        public virtual SPORT SPORT1 { get; set; }
        public virtual ICollection<USERQUAL> USERQUALs { get; set; }
    }
}
