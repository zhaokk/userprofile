﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Script.Serialization;
using userprofile.Models;
using userprofile.Models.Serializable;

namespace userprofile.Controllers
{
    /// <summary>
    /// The match api controller
    /// 
    /// to use the first variable needs to be the api key for the account you want information about
    /// 
    /// 
    /// To get upcomming matches of intrest:
    /// /api/MatchApi/*api-key*
    /// This will return matches that you referee or play in
    /// 
    /// To get a specific match details:
    /// /api/MatchApi/*api-key*?id=*match-id*
    /// This will return a single object that is the match you want
    /// 
    /// To modify a match:
    /// 
    /// </summary>
    public class MatchApiController : ApiController
    {
        private Raoconnection db = new Raoconnection();

        // GET: api/MatchApi

        public IQueryable<MATCH> GetMATCHes()
        {

            var matches = db.MATCHes.Where(match => match.status > 0);

            return matches;
        }

        public IHttpActionResult GetMATCHes(String id)
        {
            Boolean somethingSet = false;
            var userId = db.APIKEYs.Find(id);
            UpcommingMatches matMod = new UpcommingMatches();

            if (userId != null)
            {
                AspNetUser user = db.AspNetUsers.Where(m => m.Id == userId.userId).Include(m => m.PLAYERs).Include(m => m.TEAMs).FirstOrDefault();

                foreach (var role in user.AspNetRoles)
                {
                    switch (role.Name)
                    {
                        case "Referee":
                            var matches = new List<MATCH>();

                            foreach (var referee in user.REFEREEs.ElementAt(0).OFFERs)
                            {
                                matches.AddRange(db.MATCHes.Where(match => match.status == 1).Where(match => match.matchId == referee.matchId).ToList());
                            }

                            if (matches.Count > 0)
                            {
                                matMod.myUpcommingReferee = matches.ToList();
                                somethingSet = true;
                            }

                            break;


                        case "Player":
                                var playerMatches = new List<MATCH>();
                                foreach(var player in user.PLAYERs){
                                    playerMatches.AddRange(db.MATCHes.Where(match => match.status == 1).Where(match => match.teamAId == player.teamId || match.teamBId == player.teamId).ToList());
                                }

                                if (playerMatches.Count > 0)
                                {
                                    matMod.myUpcommingPlayer = playerMatches.ToList();
                                    somethingSet = true;
                                }
                                break;


                        case "Organizer":
                                var organizerMatches = db.MATCHes.Where(match => match.status > 0);

                                if (organizerMatches != null)
                                {
                                   // matMod.myUpcomming = organizerMatches.ToList();
                                    somethingSet = true;
                                }
                                break;


                        default:
                            break;

                    }
                }
            }

            //DateTime nextFortnight = DateTime.Now;
            //nextFortnight = nextFortnight.AddDays(14);

            if (somethingSet)
                return Ok(matMod);
            return NotFound();
        }


        // GET: api/Mobile/5
        [ResponseType(typeof(MATCH))]
        public async Task<IHttpActionResult> GetMATCH(String id, int matchId)
        {
            var userId = db.APIKEYs.Find(id);

            if (userId != null)
            {

                MATCH match = await db.MATCHes.FindAsync(matchId);
                if (match == null)
                {
                    return NotFound();
                }
                return Ok(match);
            }

            return NotFound();
        }

        /// <summary>
        /// Takes an api key, match id and match object to UPDATE
        /// validates and updates
        /// </summary>
        /// <param name="id">api key</param>
        /// <param name="matchId">id of the match to update</param>
        /// <param name="match">the match object to set</param>
        /// <returns>BadRequest, NotFound or a status code depending on what happens</returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMATCH(String id, int matchId, MATCH match)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = db.APIKEYs.Find(id);

            if (userId != null)
            {
                if (matchId != match.matchId)
                {
                    return BadRequest();
                }

                db.Entry(match).State = EntityState.Modified;

                try
                {
                    await db.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MATCHExists(matchId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }


            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        /// <summary>
        /// takes an api key and a match object to ADD to the database
        /// </summary>
        /// <param name="id">api key</param>
        /// <param name="match">a MATCH object to set</param>
        /// <returns></returns>
        [ResponseType(typeof(MATCH))]
        public async Task<IHttpActionResult> PostMATCH(String id, MATCH match)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = db.APIKEYs.Find(id);

            if (userId != null)
            {
                db.MATCHes.Add(match);
                await db.SaveChangesAsync();
            }


            return CreatedAtRoute("DefaultApi", new { id = match.matchId }, match);
        }

        /// <summary>
        /// clears the database variable
        /// </summary>
        /// <param name="disposing">true if the database connection needs taking care of</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        /// <summary>
        /// Checks if the given match exists
        /// </summary>
        /// <param name="id">match id to look for</param>
        /// <returns>true if exists</returns>
        private bool MATCHExists(int id)
        {
            return db.MATCHes.Count(e => e.matchId == id) > 0;
        }
    }
}