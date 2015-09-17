using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class matchViewModel
    {
       [Display(Name = "Offer Number")]
        public int offernum { get; set; }
        [Display(Name = "Created Match")]
        public MATCH createdMatch { get; set; }
      //  public LOCATION newlocation { get; set; }

        public Nullable<int> q1 { get; set; }
        
        public Nullable<int> q2 { get; set; }
        public Nullable<int> q3 { get; set; }
        public Nullable<int> ql1 { get; set; }

        public Nullable<int> ql2 { get; set; }
        public Nullable<int> ql3 { get; set; }
        [Display(Name = "Offers")]
        public List<OFFER> offers { get; set; }
         [Required(ErrorMessage = "type1 is required")]
        public string type1 { get; set; }
         [Required(ErrorMessage = "Type2 is required")]
        public string type2 { get; set; }
         [Required(ErrorMessage = "Type3 is required")]
        public string type3 { get; set; }
        
    }
    
}