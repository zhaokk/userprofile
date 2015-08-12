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
            this.offers = new List<OFFER>();
            if (ma.OFFERs != null) {
                foreach (OFFER of in ma.OFFERs)
                {
                    this.offers.Add(of);

                }
                if (ma.OFFERs.Count() != 2)
                {
                    for (int i = (3 - ma.OFFERs.Count()); i > 0; i--)
                    {
                        OFFER fakeOffer = new OFFER();
                        fakeOffer.status = "dummy";
                        this.offers.Add(fakeOffer);

                    }
                
                }
            }
            this.mID = ma.mID;
        }

        public List<OFFER> offers { get; set; }
        public int mID { get; set; }
    }

  
}