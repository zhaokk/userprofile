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
    public class PlayerController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: PLAYERs
        public ActionResult Index()
        {
            var players = db.PLAYERs.Include(p => p.AspNetUser).Include(p => p.TEAM).Where(player => player.status > 0).ToList();
            return View(players);
        }

        // GET: PLAYERs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER player = db.PLAYERs.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // GET: PLAYERs/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name");
            return View();
        }

        // POST: PLAYERs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "teamId,userId,position,shirtNum,status")] PLAYER player)
        {
            player.status = 1;
            if (ModelState.IsValid)
            {
				IdentityManager idManager = new IdentityManager();
				idManager.AddUserToRole(player.AspNetUser.Id, "Player");
                db.PLAYERs.Add(player);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", player.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", player.teamId);
            return View(player);
        }

        // GET: PLAYERs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER player = db.PLAYERs.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", player.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", player.teamId);
            return View(player);
        }

        // POST: PLAYERs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "teamId,userId,position,shirtNum,status")] PLAYER player)
        {
            if (ModelState.IsValid)
            {
                db.Entry(player).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", player.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", player.teamId);
            return View(player);
        }

        // GET: PLAYERs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER player = db.PLAYERs.Find(id);
            if (player == null)
            {
                return HttpNotFound();
            }
            return View(player);
        }

        // POST: PLAYERs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PLAYER player = db.PLAYERs.Find(id);

            player.status = 0;
			bool remove = true;
			foreach (var i in player.AspNetUser.PLAYERs) { //remove from role if no active player
				if (i.status > 0 && i.teamId != player.teamId) {
					remove = false;
					break;
				}
			}
			if (remove) {
				IdentityManager idManager = new IdentityManager();
				idManager.RemoveUserFromRole(player.AspNetUser.Id, "Player");
			}
            db.Entry(player).State = EntityState.Modified;

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
