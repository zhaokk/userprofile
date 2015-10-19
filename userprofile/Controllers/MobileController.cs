using System;
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
using userprofile.Models;

namespace userprofile.Controllers
{
    public class MobileController : ApiController
    {
        private Raoconnection db = new Raoconnection();

        // GET: api/Mobile
        [Route("api/matches")]
        public IQueryable<MATCH> GetMATCHes()
        {
            Debug.Write("g");
            DateTime nextFortnight = DateTime.Now;
            nextFortnight = nextFortnight.AddDays(14);

            var matches = db.MATCHes.Where(match => match.status > 0);

            return matches;
        }

        // GET: api/Mobile/5
        [ResponseType(typeof(MATCH))]
        public async Task<IHttpActionResult> GetMATCH(int id)
        {
            MATCH mATCH = await db.MATCHes.FindAsync(id);
            if (mATCH == null)
            {
                return NotFound();
            }

            return Ok(mATCH);
        }

        // PUT: api/Mobile/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMATCH(int id, MATCH mATCH)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mATCH.matchId)
            {
                return BadRequest();
            }

            db.Entry(mATCH).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MATCHExists(id))
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

        // POST: api/Mobile
        [ResponseType(typeof(MATCH))]
        public async Task<IHttpActionResult> PostMATCH(MATCH mATCH)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.MATCHes.Add(mATCH);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = mATCH.matchId }, mATCH);
        }

        // DELETE: api/Mobile/5
        [ResponseType(typeof(MATCH))]
        public async Task<IHttpActionResult> DeleteMATCH(int id)
        {
            MATCH mATCH = await db.MATCHes.FindAsync(id);
            if (mATCH == null)
            {
                return NotFound();
            }

            db.MATCHes.Remove(mATCH);
            await db.SaveChangesAsync();

            return Ok(mATCH);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MATCHExists(int id)
        {
            return db.MATCHes.Count(e => e.matchId == id) > 0;
        }
    }
}