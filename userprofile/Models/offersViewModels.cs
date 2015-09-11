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
                if (ma.OFFERs.Count() != 3)
                {
                    for (int i = (3 - ma.OFFERs.Count()); i > 0; i--)
                    {
                        OFFER fakeOffer = new OFFER();
                        fakeOffer.status = 5;
                        this.offers.Add(fakeOffer);

                    }
                
                }
            }
            this.mID = ma.matchId;
        }

        public List<OFFER> offers { get; set; }
        public int mID { get; set; }
    }

    public class shortoffer
    {
    public shortoffer(OFFER of){
    
    }
        public shortoffer(){
        
        }
    public int matchId {get;set;}
    public int refId {get;set;}
    public string status {get;set;}
    
    }
    public class offerRefereeViewModels
    {
        public offerRefereeViewModels() { }
        public offerRefereeViewModels(REFEREE refe){
             this.offers = refe.OFFERs.ToList();


    
    }
        public List<OFFER> offers { get; set; }
    
    
    }
}