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
    public class RefereeController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: /Referee/
        public ActionResult Index()
        {
            var referees = db.REFEREEs.Include(r => r.AspNetUser).Include(r => r.SPORT1);
            return View(referees.ToList());
        }

        // GET: /Referee/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REFEREE referee = db.REFEREEs.Find(id);
            if (referee == null)
            {
                return HttpNotFound();
            }
            return View(referee);
        }

        // GET: /Referee/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            return View();
        }

        // POST: /Referee/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="refID,availability,distTravel,sport,prefAge,prefGrade,ID")] REFEREE referee)
        {
            if (ModelState.IsValid)
            {
                db.REFEREEs.Add(referee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName", referee.ID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", referee.sport);
            return View(referee);
        }

        // GET: /Referee/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REFEREE referee = db.REFEREEs.Find(id);
            if (referee == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName", referee.ID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", referee.sport);
            return View(referee);
        }

        // POST: /Referee/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="refID,availability,distTravel,sport,prefAge,prefGrade,ID")] REFEREE referee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(referee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName", referee.ID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", referee.sport);
            return View(referee);
        }

        // GET: /Referee/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REFEREE referee = db.REFEREEs.Find(id);
            if (referee == null)
            {
                return HttpNotFound();
            }
            return View(referee);
        }

        // POST: /Referee/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            REFEREE referee = db.REFEREEs.Find(id);
            db.REFEREEs.Remove(referee);
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
