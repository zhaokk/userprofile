﻿using System;
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
    /// deals with pending offers
    /// {id} is an apiKey
    /// </summary>
    public class PendingOffersController : ApiController
    {
        private Raoconnection db = new Raoconnection();

        /// <summary>
        /// Default get, does nothing
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetOFFERs()
        {
            return NotFound();
        }

        /// <summary>
        /// Returns the offers awaiting response from this referee
        /// </summary>
        /// <param name="id">The api key</param>
        /// <returns>A json formatted array containing rejected offers</returns>
        public IQueryable<OFFER> GetOFFERs(String id)
        {

            var userId = db.APIKEYs.Find(id);


            if (userId != null)
            {

                var tempOffers = db.OFFERs.Include(offer => offer.MATCH).Include(offer => offer.MATCH.LOCATION)
                                          .Where(offer => offer.REFEREE.userId == userId.userId)
                                          .Where(offer => offer.status == 3);
                  
                foreach (var offers in tempOffers)
                {
                    offers.nameOfLocation = offers.MATCH.LOCATION.name;
                    offers.dateOfMatch = offers.MATCH.matchDate;
                }

                return tempOffers;
            }
            return null;
        }

        /// <summary>
        /// Gets a specific offer
        /// </summary>
        /// <param name="id">apiKey</param>
        /// <param name="offerId">The offerId</param>
        /// <returns>a single offer</returns>
        [ResponseType(typeof(OFFER))]
        public async Task<IHttpActionResult> GetOFFER(String id, int offerId)
        {
            var userId = db.APIKEYs.Find(id);

            if (userId != null)
            {

                OFFER offer = await db.OFFERs.FindAsync(id);
                if (offer != null)
                {
                    return Ok(offer);
                }
            }

            return NotFound();
        }

        /// <summary>
        /// not implemented
        /// </summary>
        /// <param name="id"></param>
        /// <param name="oFFER"></param>
        /// <returns></returns>
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutOFFER(int id, OFFER oFFER)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != oFFER.offerId)
            {
                return BadRequest();
            }

            db.Entry(oFFER).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OFFERExists(id))
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


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool OFFERExists(int id)
        {
            return db.OFFERs.Count(e => e.offerId == id) > 0;
        }
    }
}