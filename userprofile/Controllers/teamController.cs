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
    public class teamController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: /team/
        public ActionResult Index()
        {
            var teams = db.TEAMs;
            ViewBag.breadcrumbs = "list of team";
            List<TEAM> teamlist = teams.ToList();
            List<TEAM> teamintournament = new List<TEAM>();
            if (Session["tournamentID"] != null)
            {
                string tid = Session["tournamentID"] as string;
                int tournamentID = Int32.Parse(tid);
                foreach (var team in teamlist)
                {
                    foreach (var teamin in team.TEAMINS) {
                        if (teamin.tournament == tournamentID)
                        {
                            teamintournament.Add(team);
                        }
                    
                    }
               
                }

            }
            else
            {
                teamintournament = teamlist;

            }
            return View(teamintournament);
        }

        // GET: /team/Details/5
        public ActionResult Details(int? id)
        {
            DateTime nextWeek = DateTime.Today;
            nextWeek = nextWeek.AddDays(14);

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEAM team = db.TEAMs.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            List<TEAMIN> pastTournaments = db.TEAMINS.Where(pastTournament => pastTournament.teamID == id).Where(pastTournament => pastTournament.TOURNAMENT1.status == 2).ToList();
            if (team == null)
            {
                return HttpNotFound();
            }

            List<MATCH> matches = db.MATCHes.Where(match => match.status == 1).Where(match => match.teamAId == id || match.teamBId == id).ToList();



            var playerIn = db.PLAYERs.Where(teams => teams.teamId == id).ToList();

            var combined = new Tuple<TEAM, List<PLAYER>, List<TEAMIN>, List<MATCH>>(team, playerIn, pastTournaments, matches) { };

            return View(combined);
        }

        // GET: /team/Create
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Create()
        {
            ViewBag.managerID = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "name"); //need to pass tournamentId arround
            return View();
        }

        // POST: /team/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Create([Bind(Include="teamId,name,ageBracket,grade,managerId,shortName,status")] TEAM team)
        {
            team.sport = "Soccer";
            team.status = 1;

            if (ModelState.IsValid)
            {

                // need to pass a tournament in here for adding to the team, create for now with default tournament
                /*TOURNAMENT t = db.TOURNAMENTs.Find(1);
                
                var tins = new TEAMIN();
                tins.tournament = t.tournamentId;
                team.TEAMINS.Add(tins);*/
				new IdentityManager().AddUserToRole(team.managerId, "Manager");
                db.TEAMs.Add(team);
                db.SaveChanges();
                return RedirectToAction("Details", "Team", new {Id = team.teamId });
            }

            ViewBag.managerId = new SelectList(db.AspNetUsers, "Id", "UserName", team.managerId);
           
            return View(team);
        }

        // GET: /team/Edit/5
        [Authorize(Roles = "Admin,Organizer,Manager")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEAM team = db.TEAMs.Find(id);
            db.Entry(team).Reference(r => r.SPORT1).Load();

            if (team == null)
            {
                return HttpNotFound();
            }
            ViewBag.managerId = new SelectList(db.AspNetUsers, "Id", "UserName", team.managerId);
           
            return View(team);
        }

        // POST: /team/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Edit([Bind(Include = "teamId,name,ageBracket,grade,managerId,sport,shortName,status")] TEAM team)
        {
            if (ModelState.IsValid)
            {
				TEAM old = db.TEAMs.Find(team.teamId);
				if (db.TEAMs.Find(team.teamId).managerId != team.managerId) { //check if manager changed
					bool remove = true;
					foreach (var i in db.AspNetUsers.Find(old.managerId).TEAMs) { //check if manages another active team
						if (i.status > 0 && i.teamId != team.teamId) {
							remove = false;
							break;
						}
					}
					if (remove) { //if not then remove role
						new IdentityManager().RemoveUserFromRole(old.managerId, "Manager");
					}
					new IdentityManager().AddUserToRole(team.managerId, "Manager"); //add role to new manager
				}
				
                db.Entry(team).State = System.Data.Entity.EntityState.Modified;
                //db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.managerId = new SelectList(db.AspNetUsers, "Id", "UserName", team.managerId);
           
            return View(team);
        }

        // GET: /team/Delete/5
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TEAM team = db.TEAMs.Find(id);
            if (team == null)
            {
                return HttpNotFound();
            }
            return View(team);
        }

        // POST: /team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult DeleteConfirmed(int id)
        {
            TEAM team = db.TEAMs.Find(id);
            db.TEAMs.Remove(team);
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
