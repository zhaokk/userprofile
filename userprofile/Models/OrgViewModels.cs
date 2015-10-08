using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    
    public class OrgViewModels
    {
        public OrgViewModels(Raoconnection db,string userid) {
            List<OFFER> offermanagedbyO=new List<OFFER>();
           AspNetUser Orgnizer= db.AspNetUsers.Find(userid);
          majorT=db.TOURNAMENTs.First(d => d.organizer == userid);
            //posible other tournamanes
         foreach(MATCH m in  majorT.MATCHes){
             foreach (OFFER o in m.OFFERs) {
                 offermanagedbyO.Add(o);
             }
         }
       orgOVM = new admineOfferViewModel(offermanagedbyO);
        }
        public OrgViewModels() {
        }


        public TOURNAMENT majorT { get; set; }
        public List<TOURNAMENT> otherTs { get; set; }

        public admineOfferViewModel orgOVM { get; set; }
    }
}