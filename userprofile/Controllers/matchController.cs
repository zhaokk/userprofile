using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;

namespace userprofile.Controllers
{
    public class matchController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: /match/
        public ActionResult Index()
        {
            var matches = db.MATCHes.Include(m => m.LOCATION1).Include(m => m.TEAM).Include(m => m.TEAM1).Include(m => m.TOURNAMENT1);
            return View(matches.ToList());
        }

        // GET: /match/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATCH match = db.MATCHes.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // GET: /match/Create
        public ActionResult Create()
        {
            ViewBag.location = new SelectList(db.LOCATIONs, "lID", "name");
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamID", "name");
            ViewBag.teambID = new SelectList(db.TEAMs, "teamID", "name");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport");
            return View();
        }

        // POST: /match/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="mID,matchDate,location,teamaID,teambID,winnerID,tournament")] MATCH match)
        {
            DateTime myDate = DateTime.ParseExact(Request.QueryString["datetime"], "yyyy-MM-dd HH:mm:ss",
                                       System.Globalization.CultureInfo.InvariantCulture);
            match.matchDate = myDate;
            

            if (ModelState.IsValid)
            {
                db.MATCHes.Add(match);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.location = new SelectList(db.LOCATIONs, "lID", "name", match.location);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamID", "name", match.teamaID);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamID", "name", match.teambID);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport", match.tournament);
            return View(match);
        }

        // GET: /match/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATCH match = db.MATCHes.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.location = new SelectList(db.LOCATIONs, "lID", "name", match.location);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamID", "name", match.teamaID);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamID", "name", match.teambID);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport", match.tournament);
            return View(match);
        }

        // POST: /match/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="mID,matchDate,location,teamaID,teambID,winnerID,tournament")] MATCH match)
        {
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.location = new SelectList(db.LOCATIONs, "lID", "name", match.location);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamID", "name", match.teamaID);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamID", "name", match.teambID);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport", match.tournament);
            return View(match);
        }

        // GET: /match/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATCH match = db.MATCHes.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // POST: /match/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MATCH match = db.MATCHes.Find(id);
            db.MATCHes.Remove(match);
            db.SaveChanges();
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
