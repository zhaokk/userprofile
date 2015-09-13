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
    public class qualificationController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: /qualification/
        public ActionResult Index()
        {
            var qualifications = db.QUALIFICATIONS.Include(q => q.SPORT1);
            return View(qualifications.ToList());
        }

        // GET: /qualification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // GET: /qualification/Create
        public ActionResult Create()
        {
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            return View();
        }

        // POST: /qualification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "qualificationId,name,sport,description")] QUALIFICATION qualification)
        {
            if (ModelState.IsValid)
            {
                db.QUALIFICATIONS.Add(qualification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", qualification.sport);
            return View(qualification);
        }

        // GET: /qualification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            db.Entry(qualification).Reference(r => r.SPORT1).Load();
            if (qualification == null)
            {
                return HttpNotFound();
            }
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", qualification.sport);
            return View(qualification);
        }

        // POST: /qualification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "qualificationId,name,sport,description,qualificationLevel")] QUALIFICATION qualification)
        {
            if (ModelState.IsValid)
            {
                //qualification.SPORT1 = db.SPORTs.Find(qualification.sport);
                db.Entry(qualification).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", qualification.sport);
            return View(qualification);
        }

        // GET: /qualification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // POST: /qualification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            db.QUALIFICATIONS.Remove(qualification);
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
