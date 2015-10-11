using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace userprofile.Models
{

    public class managerViewModels
    {

        public managerViewModels() { }

        public managerViewModels(Raoconnection db,string userid) {
            teams = db.TEAMs.Where(t => t.managerId == userid).ToList();
          // teams= db.TEAMs.All(t => t.managerId == userid);
        }
        public List<TEAM> teams { get; set; }
    }
}