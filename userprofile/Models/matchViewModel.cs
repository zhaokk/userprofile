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

        public Nullable<int> q1 { get; set; }
        
        public Nullable<int> q2 { get; set; }
        public Nullable<int> q3 { get; set; }
        public Nullable<int> ql1 { get; set; }

        public Nullable<int> ql2 { get; set; }
        public Nullable<int> ql3 { get; set; }
        public List<OFFER> offers { get; set; }
        public string type1 { get; set; }
        public string type2 { get; set; }
        public string type3 { get; set; }
        
    }
    
}