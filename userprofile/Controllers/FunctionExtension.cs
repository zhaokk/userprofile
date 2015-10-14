using System;
using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Linq;
using System.Web;

namespace userprofile.Controllers
{
    public class FunctionExtension
    {

        public static DbGeography CreatePoint(double lat, double lon)
        {
            string wkt = String.Format("POINT({0} {1})", lat, lon);

            return DbGeography.FromText(wkt);
        }

       
    }
}