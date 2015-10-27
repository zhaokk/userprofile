using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;
using Microsoft.AspNet.Identity;

namespace userprofile.Controllers
{
    public class OneOffAVAILABILITiesController : Controller
    {
        private Raoconnection db = new Raoconnection();

		// GET: OneOffAVAILABILITies
		[Authorize(Roles = "Admin,Organizer,Referee")]
        public ActionResult Index()
        {
			var currentRefereeId= User.Identity.GetUserId();
            var refe= db.REFEREEs.First(rid=>rid.userId==currentRefereeId);
            var oneOffAVAILABILITies = db.OneOffAVAILABILITies.Where(o => o.refId == refe.refId).Include(o => o.REFEREE);
            return View(oneOffAVAILABILITies.ToList());
        }

        // GET: OneOffAVAILABILITies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OneOffAVAILABILITY oneOffAVAILABILITY = db.OneOffAVAILABILITies.Find(id);
            if (oneOffAVAILABILITY == null)
            {
                return HttpNotFound();
            }
            return View(oneOffAVAILABILITY);
        }

        // GET: OneOffAVAILABILITies/Create
        public ActionResult Create()
        {
            ViewBag.refId = new SelectList(db.REFEREEs, "refId", "sport");
            return View();
        }

        // POST: OneOffAVAILABILITies/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "availabilityId,refId,startDate,timeOnOrOff,description")] OneOffAVAILABILITY oneOffAVAILABILITY)
        {
            if (ModelState.IsValid)
            {
                db.OneOffAVAILABILITies.Add(oneOffAVAILABILITY);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.refId = new SelectList(db.REFEREEs, "refId", "sport", oneOffAVAILABILITY.refId);
            return View(oneOffAVAILABILITY);
        }

        // GET: OneOffAVAILABILITies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OneOffAVAILABILITY oneOffAVAILABILITY = db.OneOffAVAILABILITies.Find(id);
            if (oneOffAVAILABILITY == null)
            {
                return HttpNotFound();
            }
            ViewBag.refId = new SelectList(db.REFEREEs, "refId", "sport", oneOffAVAILABILITY.refId);
            return View(oneOffAVAILABILITY);
        }

        // POST: OneOffAVAILABILITies/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "availabilityId,refId,startDate,timeOnOrOff,description")] OneOffAVAILABILITY oneOffAVAILABILITY)
        {
            if (ModelState.IsValid)
            {
                db.Entry(oneOffAVAILABILITY).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.refId = new SelectList(db.REFEREEs, "refId", "sport", oneOffAVAILABILITY.refId);
            return View(oneOffAVAILABILITY);
        }

        // GET: OneOffAVAILABILITies/Delete/5
        public ActionResult Delete(int? id,System.DateTime start)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OneOffAVAILABILITY oneOffAVAILABILITY = db.OneOffAVAILABILITies.Find(id,start);
            if (oneOffAVAILABILITY == null)
            {
                return HttpNotFound();
            }
            return View(oneOffAVAILABILITY);
        }

        // POST: OneOffAVAILABILITies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id, System.DateTime start)
        {
            OneOffAVAILABILITY oneOffAVAILABILITY = db.OneOffAVAILABILITies.Find(id,start);
            db.OneOffAVAILABILITies.Remove(oneOffAVAILABILITY);
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
