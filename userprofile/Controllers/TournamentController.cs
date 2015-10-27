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
    public class TournamentController : Controller
    {
        private Raoconnection db = new Raoconnection();
         [HttpPost]
        public string  TournamentSelected(string tournamentID)
        {
            if (tournamentID == "")
            {
                Session["tournamentID"] = null;
                Session["tournamentname"] = null;
            }
            else { 
                Session["tournamentID"] = tournamentID;
                int tid = Int32.Parse(tournamentID);
            Session["tournamentname"] = db.TOURNAMENTs.Find(tid).name;
               }
           return ("tournamentID has change to"+ tournamentID);
        }
        // GET: TOURNAMENTs
        public async Task<ActionResult> Index()
        {
            ViewBag.session = Session["tournamentID"] as string;
            var tournament = db.TOURNAMENTs.Include(t => t.AspNetUser).Include(t => t.SPORT1).Where(tour => tour.status == 1);
            ViewBag.TournamentList = new SelectList(db.TOURNAMENTs, "tournamentId", "name", Session["tournamentID"]);
            
            return View(await tournament.ToListAsync());
        }
        public ActionResult History() {
            var oldTour = db.TOURNAMENTs.Where(d => d.status == 2).ToList();
            return View(oldTour);
        
        
        }


        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            if (tournament != null)
            {
                DateTime today = DateTime.Today;


                var futureMatches = db.MATCHes.Where(match => match.status > 0)
                                              .Where(match => match.matchDate > today).ToList();

                var pastMatches = db.MATCHes.Where(match => match.status >= 0)
                                              .Where(match => match.matchDate < today).ToList();




                var combined = new Tuple<TOURNAMENT, List<MATCH>, List<MATCH>>(tournament, futureMatches, pastMatches) { };

                return View(combined);
            }
            return HttpNotFound();
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
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "tournamentId,name,startDate,organizer,ageBracket,grade,sport,status,priority")] TOURNAMENT tournament)
        {
            tournament.status = 1;
            if (ModelState.IsValid)
            {
				
				AspNetUser organizer = db.AspNetUsers.Find(tournament.organizer); //get organizer
				//if (organizer.AspNetRoles.Where(roles => roles.Id == "4").Count() == 0) { //check if already is in organizer role
				tournament.AspNetUser = organizer;
				tournament.AspNetUsers.Add(organizer);
				IdentityManager idManager = new IdentityManager();
				idManager.AddUserToRole(organizer.Id, "Organizer"); //add user as organizer
				//db.TOURNAMENTs.Find(tournament.tournamentId).AspNetUsers.Add(tournament.AspNetUser);
				//}
				db.TOURNAMENTs.Add(tournament);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tournament.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tournament.sport);
            return View(tournament);
        }

        // GET: TOURNAMENTs/Edit/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tournament.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tournament.sport);
            return View(tournament);
        }

        // POST: TOURNAMENTs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult> Edit([Bind(Include = "tournamentId,name,startDate,organizer,ageBracket,grade,sport,status,priority")] TOURNAMENT tournament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournament).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tournament.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tournament.sport);
            return View(tournament);
        }

        // GET: TOURNAMENTs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: TOURNAMENTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            

            tournament.status = 0;
			var idManager = new IdentityManager();
			foreach (var i in tournament.AspNetUsers) { //need to remove users from organizer role if this is their only tournamnet
				bool removeRole = true;
				foreach (var k in i.TOURNAMENTs1) { //go through tournaments they organize
					if (k.status > 0 && k.tournamentId != tournament.tournamentId) { //if they have an active tournament
						removeRole = false; //don't remove the role
					}
				}
				if (removeRole) {
					idManager.RemoveUserFromRole(i.Id, "Organizer");
				}
			}
            db.Entry(tournament).State = EntityState.Modified;
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
