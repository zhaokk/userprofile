using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class offersViewModels
    {
        public offersViewModels()
        {
        
        }
        public offersViewModels(MATCH ma)
        {
            foreach (OFFER of in ma.OFFERs)
            {
                this.offers.Add(of);

            }
            this.mID = ma.mID;
        }

        public List<OFFER> offers { get; set; }
        public int mID { get; set; }
    }
}