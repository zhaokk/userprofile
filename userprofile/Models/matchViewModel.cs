using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class matchViewModel
    {
       
        public int offernum { get; set; }
        public MATCH createdMatch { get; set; }
      //  public LOCATION newlocation { get; set; }

        public int?  q1 { get; set; }
        public int?  q2 { get; set; }
        public int? q3 { get; set; }
     
        
    }
    
}