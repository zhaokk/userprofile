using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class MmatchViewModel
    {
        //public MmatchViewModel() { 
        //this.offers=new offerpair[10];
        //}
        [Display(Name = "Offer Number")]
        public int offernum { get; set; }
        [Display(Name = "Created Match")]
        public MATCH createdMatch { get; set; }
        //  public LOCATION newlocation { get; set; }

        public offerpair[] offers { get; set; }
    }
    public class offerpair
    {
        public int q { get; set; }
        public string type { get; set; }

        public int level { get; set; }
    }
   public class managerViewModels
    {
        public List<MATCH> comingMatch { get; set; }

        public List<MATCH> passMatch { get; set; }

        public List<TEAM> teamManaging { get; set; }
    }

}