using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using userprofile.Models;

namespace userprofile.Models
{
    public class AlgorithmModel
    {
        public List<solution> result { get; set; }

        public AlgorithmModel()
        {
            this.result = new List<solution>();
        }

        public AlgorithmModel(int count)
        {
            result = new List<solution>();
            for (int i = 0; i < count; i++)
            {
                result.Add(new solution());
            }
        }
        public void sortResult() {
            foreach (solution sol in result) {

                List<pair> orderedPair = sol.pairs.OrderBy(p => p.matchid).ToList();
                int cache=0;
                int flag=0;
                sorted sortedcacche = new sorted();
                foreach (pair p in orderedPair) {
                    if (p.matchid != cache)
                    {
                        sorted sortedone = new sorted();
                        sortedone.assigned.Add(p);
                        sortedone.match = p.match;
                        
                        
                        cache = p.matchid;
                        if(flag!=0){
                        sol.matchpair.Add(sortedcacche);
                        }
                        sortedcacche = sortedone;
                        flag++;
                    }
                    else {
                        sortedcacche.assigned.Add(p);
                    
                    }
                }
                sol.matchpair.Add(sortedcacche);
            
            }
        
        }

    }
    public class solution
    {
        public List<pair> pairs { get; set; }
        public List<sorted> matchpair { get; set; }
        public solution()
        {
            pairs = new List<pair>();
            matchpair = new List<sorted>();
        }

    }
    public class sorted
    {
        public MATCH match { get; set; }
        public List<pair> assigned { get; set; }

        public sorted()
        {
            this.match = new MATCH();
            this.assigned = new List<pair>();

        }
    }

    public class pair
    {
        public pair(int offer, int referee, Raoconnection db)
        {
            if (referee == -1)
            {
                offerid = offer;
                this.refeid = -1;
                this.refe = db.REFEREEs.Find(87784161);
                this.offer = db.OFFERs.Find(offer);
                this.match = this.offer.MATCH;
                this.matchid = this.match.matchId;
            }
            else
            {

                this.offerid = offer;
                this.refeid = referee;
                this.refe = db.REFEREEs.Find(referee);
                this.offer = db.OFFERs.Find(offer);
                this.match = this.offer.MATCH;
                this.matchid = this.match.matchId;
            }

        }
        public pair() { }
        public MATCH match { get; set; }
        public REFEREE refe { get; set; }
        public OFFER offer { get; set; }
        public int refeid { get; set; }
        public int offerid { get; set; }
        public int matchid { get; set; }
    }


}