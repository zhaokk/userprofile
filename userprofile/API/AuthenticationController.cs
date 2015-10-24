using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using userprofile.Models;
using userprofile.Controllers;

namespace userprofile.mobile
{
    public class AuthenticationController : ApiController
    {
        private Raoconnection db = new Raoconnection();


        /// <summary>
        /// Checks if a user has an api key, if so it gives it back for use later on
        /// </summary>
        /// <param name="id">The username</param>
        /// <param name="password">The password</param>
        /// <returns>a json formated string containing the api key, -1 if user does not exist</returns>
        public HttpResponseMessage GetApiKey(String id, String password)
        {

            var im = new IdentityManager();


            var response = this.Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent("{\"apiKey\":\"-1\"}", Encoding.UTF8, "application/json");


            if (true)
            {
                var user = db.AspNetUsers.Where(m => m.UserName == id).FirstOrDefault();

                if (user.APIKEYs != null && user.APIKEYs.Count > 0)
                {
                    response.Content = new StringContent("{\"apiKey\":\"" + user.APIKEYs.ElementAt(0).ID + "\"}", Encoding.UTF8, "application/json");
                }
                else
                {

                    var key = new APIKEY();
                    key.ID = Guid.NewGuid().ToString();
                    user.APIKEYs.Add(key);
                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
                    response.Content = new StringContent("{\"apiKey\":\"" + user.APIKEYs.ElementAt(0).ID + "\"}", Encoding.UTF8, "application/json");
                }
            }


            return response;
        }


    }
}
