using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace userprofile.Controllers
{
    class managerViewModels
    {
        private Models.Raoconnection db;
        private string userID;

        public managerViewModels(Models.Raoconnection db, string userID)
        {
            // TODO: Complete member initialization
            this.db = db;
            this.userID = userID;
        }
    }
}
