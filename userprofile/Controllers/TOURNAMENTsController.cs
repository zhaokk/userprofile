using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;

namespace userprofile.Controllers
{
    public class TOURNAMENTsController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: TOURNAMENTs
        public async Task<ActionResult> Index()
        {
            var tOURNAMENTs = db.TOURNAMENTs.Include(t => t.AspNetUser).Include(t => t.SPORT1);
            return View(await tOURNAMENTs.ToListAsync());
        }
        public ActionResult History() {
            var oldTour = db.TOURNAMENTs.Where(d => d.status == 2).ToList();
            return View(oldTour);
        
        
        }

        // GET: TOURNAMENTs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tOURNAMENT = await db.TOURNAMENTs.FindAsync(id);
            if (tOURNAMENT == null)
            {
                return HttpNotFound();
            }
            return View(tOURNAMENT);
        }

        // GET: TOURNAMENTs/Create
        public ActionResult Create()
        {
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            return View();
        }

        // POST: TOURNAMENTs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "tournamentId,name,startDate,organizer,ageBracket,grade,sport,status,priority")] TOURNAMENT tOURNAMENT)
        {
            tOURNAMENT.status = 1;
            if (ModelState.IsValid)
            {
                db.TOURNAMENTs.Add(tOURNAMENT);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tOURNAMENT.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tOURNAMENT.sport);
            return View(tOURNAMENT);
        }

        // GET: TOURNAMENTs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tOURNAMENT = await db.TOURNAMENTs.FindAsync(id);
            if (tOURNAMENT == null)
            {
                return HttpNotFound();
            }
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tOURNAMENT.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tOURNAMENT.sport);
            return View(tOURNAMENT);
        }

        // POST: TOURNAMENTs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "tournamentId,name,startDate,organizer,ageBracket,grade,sport,status,priority")] TOURNAMENT tOURNAMENT)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tOURNAMENT).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tOURNAMENT.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tOURNAMENT.sport);
            return View(tOURNAMENT);
        }

        // GET: TOURNAMENTs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tOURNAMENT = await db.TOURNAMENTs.FindAsync(id);
            if (tOURNAMENT == null)
            {
                return HttpNotFound();
            }
            return View(tOURNAMENT);
        }

        // POST: TOURNAMENTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TOURNAMENT tOURNAMENT = await db.TOURNAMENTs.FindAsync(id);
            db.TOURNAMENTs.Remove(tOURNAMENT);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
