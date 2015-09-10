using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace userprofile.Models
{
    public class AlgorithmModel
    {
        public List<solution> result { get; set; }
        public AlgorithmModel()
        {
            this.result = new List<solution>();
        }

    }
    public class solution
    {
        public List<pair> pairs { get; set; }
    }
    public class pair
    {
        public pair(int refereeID, int offerID)
        {
            this.refID = refereeID;
            this.offID = offerID;
            Entities enti = new Entities();
            this.offer = enti.OFFERs.Find(offID);
            this.refe = enti.REFEREEs.Find(refID);
        }
        public pair() { }
        int refID { get; set; }
        int offID { get; set; }
        public REFEREE refe { get; set; }
        public OFFER offer { get; set; }
    }


}