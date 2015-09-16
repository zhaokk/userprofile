﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using userprofile.Models;

namespace userprofile.Models {
    public class AlgorithmModel {
        public List<solution> result { get; set; }
        public AlgorithmModel() {
            this.result = new List<solution>();
        }

        public AlgorithmModel(int count) {
            result = new List<solution>();
            for (int i = 0; i < count; i++) {
                result.Add(new solution());
            }
        }

    }
    public class solution {
        public List<pair> pairs { get; set; }

        public solution() {
            pairs = new List<pair>();
        }
    }
    public class pair {
        public pair(int offer, int referee,Raoconnection db) {
            if (referee == -1)
            {
				offerid = offer;
				this.refeid = -1;
                this.refe = db.REFEREEs.Find(87784161);
                this.offer = db.OFFERs.Find(offer);
            }
            else {
				this.offerid = offer;
				this.refeid = referee;
                this.refe = db.REFEREEs.Find(referee);
                this.offer = db.OFFERs.Find(offer);
            }
            
        }
        public pair() { }
        public REFEREE refe { get; set; }
        public OFFER offer { get; set; }
		public int refeid { get; set; }
		public int offerid { get; set; }
    }


}