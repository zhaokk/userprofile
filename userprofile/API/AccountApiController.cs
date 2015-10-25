using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using userprofile.Models;

namespace userprofile.API
{
    /// <summary>
    /// Account related controller
    /// {id} is an api key
    /// </summary>
    public class AccountApiController : ApiController
    {
        private Raoconnection db = new Raoconnection();

        /// <summary>
        /// Dummy getter
        /// </summary>
        /// <returns>nothing</returns>
        public IQueryable<AspNetUser> GetAspNetUsers()
        {
            return null;
        }

        /// <summary>
        /// Dummy getter, we don't want people to be able to get a list of users with no context
        /// </summary>
        /// <param name="id"></param>
        /// <returns>nothing</returns>
        public IQueryable<AspNetUser> GetAspNetUsers(String id)
        {


            return null;
        }

        /// <summary>
        /// Takes an api key and a username, returns that users details
        /// </summary>
        /// <param name="id">api key</param>
        /// <param name="userId">That users unique id</param>
        /// <returns>an object of AspNetUser</returns>
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> GetAspNetUser(string id, String userId)
        {
            var user = await db.APIKEYs.Where(api => api.userId == userId).Where(api => api.ID == id).FirstOrDefaultAsync();

            if (user != null)
            {
                var userToDealWith = db.AspNetUsers.Find(userId);
                String tempPhotoDir = "";

                //strip the photo directory to what's needed and add in the url
                int pFrom = userToDealWith.photoDir.IndexOf("\\") + "\\".Length;
                tempPhotoDir = userToDealWith.photoDir.Substring(pFrom);
                pFrom = tempPhotoDir.IndexOf("\\") + "\\".Length;
                tempPhotoDir = tempPhotoDir.Substring(pFrom);
                userToDealWith.photoDir = "http://csci342.azurewebsites.net/userprofile/"+tempPhotoDir;


                return Ok(db.AspNetUsers.Find(userId));

            }

            return NotFound();
        }

        /// <summary>
        /// Updates a users details
        /// </summary>
        /// <param name="id">api key</param>
        /// <param name="userId">Unique user id</param>
        /// <param name="aspNetUser">The object to update</param>
        /// <returns>status code if good, model problems if bad</returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutAspNetUser(String id, string userId, AspNetUser aspNetUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (userId != aspNetUser.Id)
            {
                return BadRequest();
            }


            var user = await db.APIKEYs.Where(api => api.userId == userId).Where(api => api.ID == id).FirstOrDefaultAsync();

            if (user != null)
            {


                db.Entry(aspNetUser).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AspNetUserExists(userId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return StatusCode(HttpStatusCode.NoContent);

            }

            return NotFound();


        }

        // POST: api/AccountApi
        [ResponseType(typeof(AspNetUser))]
        public async Task<IHttpActionResult> PostAspNetUser(AspNetUser aspNetUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.AspNetUsers.Add(aspNetUser);


            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AspNetUserExists(aspNetUser.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtRoute("DefaultApi", new { id = aspNetUser.Id }, aspNetUser);
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool AspNetUserExists(string id)
        {
            return db.AspNetUsers.Count(e => e.Id == id) > 0;
        }
    }
}