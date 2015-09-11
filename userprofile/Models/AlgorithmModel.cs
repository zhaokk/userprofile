using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public pair(OFFER offer, REFEREE referee) {
            this.refe = referee;
            this.offer = offer;
        }
        public pair() { }
        public REFEREE refe { get; set; }
        public OFFER offer { get; set; }
    }


}