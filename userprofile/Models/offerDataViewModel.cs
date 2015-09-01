using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using userprofile.Models;

namespace userprofile.Models
{
    public class offerDataViewModel
    {
        private List<OFFER> alloffers;

        offerDataViewModel()
        { }
        public offerDataViewModel(List<OFFER> allOffer)
        {
            acceptedOffers = new List<OFFER>();
            alloffers = new List<OFFER>();
            declinedOffers = new List<OFFER>();
            pendingOffers = new List<OFFER>();
            foreach (var offer in allOffer)
            { 
            switch (offer.status){
                case 1:
                    acceptedOffers.Add(offer);
                    break;
                case 2:
                    declinedOffers.Add(offer);
                    break;
                case 3:
                    pendingOffers.Add(offer);
                    break;
                default:
                    break;

            }
            }
            
        }

    
        public List<OFFER> acceptedOffers { get; set; }
        public List<OFFER> declinedOffers { get; set; }
        public List<OFFER> pendingOffers { get; set; }
    }
    
}