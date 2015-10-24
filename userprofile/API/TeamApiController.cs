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
    /// /api/TeamApi
    /// </summary>
    public class TeamApiController : ApiController
    {
        private Raoconnection db = new Raoconnection();

        /// <summary>
        /// dummy getter
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEAM> GetTEAMs()
        {
            return null;
        }

        /// <summary>
        /// /api/TeamApi/*apiKey*
        /// returns teams that the user corosponding to the api key belongs to
        /// </summary>
        /// <param name="id">api key</param>
        /// <returns>a formatted array of teams </returns>
        public IHttpActionResult GetTEAMs(String id)
        {
            var userId = db.APIKEYs.Find(id);

            if (userId != null)
            {
                var teams = new List<TEAM>();
                var playerIn = db.PLAYERs.Include(player => player.TEAM).Where(t => t.userId == userId.userId).ToList();

                if (playerIn.Count > 0)
                {
                    foreach (var play in playerIn)
                    {
                        play.TEAM.managerName = play.TEAM.AspNetUser.firstName + " " + play.TEAM.AspNetUser.lastName;
                        teams.Add(play.TEAM);
                    }
                    return Ok(teams);
                }

            }


            return NotFound();
        }


        // GET: api/TeamApi/5
        [ResponseType(typeof(TEAM))]
        public async Task<IHttpActionResult> GetTEAM(String id, int teamId)
        {
            TEAM tEAM = await db.TEAMs.FindAsync(teamId);
            if (tEAM == null)
            {
                return NotFound();
            }

            return Ok(teamId);
        }

        // PUT: api/TeamApi/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutTEAM(int id, TEAM tEAM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != tEAM.teamId)
            {
                return BadRequest();
            }

            db.Entry(tEAM).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TEAMExists(id))
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

        // POST: api/TeamApi
        [ResponseType(typeof(TEAM))]
        public async Task<IHttpActionResult> PostTEAM(TEAM tEAM)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TEAMs.Add(tEAM);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = tEAM.teamId }, tEAM);
        }

        // DELETE: api/TeamApi/5
        [ResponseType(typeof(TEAM))]
        public async Task<IHttpActionResult> DeleteTEAM(int id)
        {
            TEAM tEAM = await db.TEAMs.FindAsync(id);
            if (tEAM == null)
            {
                return NotFound();
            }

            db.TEAMs.Remove(tEAM);
            await db.SaveChangesAsync();

            return Ok(tEAM);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TEAMExists(int id)
        {
            return db.TEAMs.Count(e => e.teamId == id) > 0;
        }
    }
}