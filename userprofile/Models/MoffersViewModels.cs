using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace userprofile.Models
{
    
    public class MoffersViewModels
    {
        public MoffersViewModels()
        {
            screeneddropdown = new List<List<SelectListItem>>();
        }
        public MoffersViewModels(MATCH ma)
        {
            screeneddropdown = new List<List<SelectListItem>>();
            this.offers = new List<OFFER>();
            if (ma.OFFERs != null) {
                foreach (OFFER of in ma.OFFERs)
                {
                    
                    if (of.status != 0) {   //not show delete offer
                        this.offers.Add(of);
                    }
                  

                }
                if (offers.Count() != 9)
                {
                    for (int i = (9 - offers.Count()); i > 0; i--)
                    {
                        OFFER fakeOffer = new OFFER();
                        fakeOffer.status = 6;
                        this.offers.Add(fakeOffer);

                    }
                
                }
            }
            this.mID = ma.matchId;
        }

        public List<OFFER> offers { get; set; }
        public int mID { get; set; }
        public List<List<SelectListItem>> screeneddropdown { get; set; }
        public int offerNumber { get; set; }
    }

    public class Mshortoffer
    {
    public Mshortoffer(OFFER of){
    
    }
        public Mshortoffer(){
        
        }
    public int matchId {get;set;}
    public int refId {get;set;}
    public string status {get;set;}
    
    }
    public class MofferRefereeViewModels
    {
        public MofferRefereeViewModels() { }
        public MofferRefereeViewModels(REFEREE refe){
             this.offers = refe.OFFERs.ToList();


    
    }
        public List<OFFER> offers { get; set; }
    
    
    }
}