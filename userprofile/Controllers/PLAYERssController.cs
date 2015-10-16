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
    public class PLAYERssController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: PLAYERss
        public ActionResult Index()
        {
            var pLAYERs = db.PLAYERs.Include(p => p.AspNetUser).Include(p => p.TEAM);
            return View(pLAYERs.ToList());
        }

        // GET: PLAYERss/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER pLAYER = db.PLAYERs.Find(id);
            if (pLAYER == null)
            {
                return HttpNotFound();
            }
            return View(pLAYER);
        }

        // GET: PLAYERss/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name");
            return View();
        }

        // POST: PLAYERss/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "teamId,userId,position,shirtNum,status")] PLAYER pLAYER)
        {
            if (ModelState.IsValid)
            {
                db.PLAYERs.Add(pLAYER);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", pLAYER.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", pLAYER.teamId);
            return View(pLAYER);
        }

        // GET: PLAYERss/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER pLAYER = db.PLAYERs.Find(id);
            if (pLAYER == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", pLAYER.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", pLAYER.teamId);
            return View(pLAYER);
        }

        // POST: PLAYERss/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "teamId,userId,position,shirtNum,status")] PLAYER pLAYER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pLAYER).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", pLAYER.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", pLAYER.teamId);
            return View(pLAYER);
        }

        // GET: PLAYERss/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER pLAYER = db.PLAYERs.Find(id);
            if (pLAYER == null)
            {
                return HttpNotFound();
            }
            return View(pLAYER);
        }

        // POST: PLAYERss/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            PLAYER pLAYER = db.PLAYERs.Find(id);
            db.PLAYERs.Remove(pLAYER);
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
