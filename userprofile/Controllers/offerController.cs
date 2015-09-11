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
    public class offerController : Controller
    {
        private Entities db = new Entities();

        // GET: /offer/
        public ActionResult Index()
        {
            var offers = db.OFFERs.Include(o => o.MATCH).Include(o => o.REFEREE).Include(o => o.SPORT1);
            return View(offers.ToList());
        }

        // GET: /offer/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OFFER offer = db.OFFERs.Find(id);
            if (offer == null)
            {
                return HttpNotFound();
            }
            return View(offer);
        }

        // GET: /offer/Create
        public ActionResult Create()
        {
            ViewBag.mid = new SelectList(db.MATCHes, "mID", "mID");
            ViewBag.refID = new SelectList(db.REFEREEs, "refID", "availability");
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            ViewBag.statusList = new List<SelectListItem>()
            {
                new SelectListItem() { Text="inactive", Value="inactive"},
        new SelectListItem() {Text="accepted", Value="accepted"},
        new SelectListItem() { Text="denied", Value="denied"},
        new SelectListItem() { Text="pending", Value="pending"},
        
      

            };
            return View();
        }

        // POST: /offer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="offerID,sport,mid,refID,status,dateOfOffer")] OFFER offer)
        {
            if (ModelState.IsValid)
            {
                db.OFFERs.Add(offer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.mid = new SelectList(db.MATCHes, "mID", "mID", offer.mid);
            ViewBag.refID = new SelectList(db.REFEREEs, "refID", "availability", offer.refID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", offer.sport);
            return View(offer);
        }

        // GET: /offer/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OFFER offer = db.OFFERs.Find(id);
            if (offer == null)
            {
                return HttpNotFound();
            }
            ViewBag.mid = new SelectList(db.MATCHes, "mID", "mID", offer.mid);
            ViewBag.refID = new SelectList(db.REFEREEs, "refID", "availability", offer.refID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", offer.sport);
            return View(offer);
        }

        // POST: /offer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="offerID,sport,mid,refID,status,dateOfOffer")] OFFER offer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(offer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.mid = new SelectList(db.MATCHes, "mID", "mID", offer.mid);
            ViewBag.refID = new SelectList(db.REFEREEs, "refID", "availability", offer.refID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", offer.sport);
            return View(offer);
        }

        // GET: /offer/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OFFER offer = db.OFFERs.Find(id);
            if (offer == null)
            {
                return HttpNotFound();
            }
            return View(offer);
        }

        // POST: /offer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OFFER offer = db.OFFERs.Find(id);
            db.OFFERs.Remove(offer);
            db.SaveChanges();
            return RedirectToAction("Index");
        
        }
        public ActionResult Accept(int offerId)
        {
           OFFER accOffer=  db.OFFERs.First(o => o.offerID == offerId);
           accOffer.status = 1;
           db.Entry(accOffer).State = EntityState.Modified;
             db.SaveChanges();
             return RedirectToAction("index", "home");
        
        }
        public ActionResult decline(int offerId) {
            OFFER decOffer = db.OFFERs.First(o => o.offerID == offerId);
            decOffer.status = 2;
            db.Entry(decOffer).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("index", "home");
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
