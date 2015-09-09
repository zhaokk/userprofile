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
        int refID { get; set; }
        int offerID { get; set; }
    }
}