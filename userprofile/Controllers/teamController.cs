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
            var teams = db.TEAMs.Include(t => t.TOURNAMENT1);
            ViewBag.breadcrumbs = "list of team";
            return View(teams.ToList());
        }

        // GET: /team/Details/5
        public ActionResult Details(int? id)
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

        // GET: /team/Create
        public ActionResult Create()
        {
            ViewBag.managerID = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport");
            return View();
        }

        // POST: /team/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="teamID,name,ageBracket,grade,managerID,tournament")] TEAM team)
        {
            if (ModelState.IsValid)
            {
                db.TEAMs.Add(team);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.managerID = new SelectList(db.AspNetUsers, "Id", "UserName", team.managerID);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport", team.tournament);
            return View(team);
        }

        // GET: /team/Edit/5
        public ActionResult Edit(int? id)
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
            ViewBag.managerID = new SelectList(db.AspNetUsers, "Id", "UserName", team.managerID);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport", team.tournament);
            return View(team);
        }

        // POST: /team/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="teamID,name,ageBracket,grade,managerID,tournament")] TEAM team)
        {
            if (ModelState.IsValid)
            {
                db.Entry(team).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.managerID = new SelectList(db.AspNetUsers, "Id", "UserName", team.managerID);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport", team.tournament);
            return View(team);
        }

        // GET: /team/Delete/5
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
