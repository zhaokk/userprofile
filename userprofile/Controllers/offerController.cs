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
        private Raoconnection db = new Raoconnection();

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


        public ActionResult AutomateAssign()
        {

           List<OFFER> offers = db.OFFERs.Where(m => m.status == 0).Include(m=> m.TYPEs).Include(m=> m.MATCH).Include(m=> m.OFFERQUALs).Include(m=> m.SPORT1).ToList();

             List<REFEREE> refs = new List<REFEREE>();
             //get referees from correct sport with related tables
             List<REFEREE> tempRefs = db.REFEREEs.Where(m => m.SPORT1.name == offers[0].SPORT1.name).Include(m => m.USERQUALs).Include(m=> m.TIMEOFF).Include(m=> m.WEEKLYAVAILABILITY).ToList();

                //look at avaliabilities only include thoes that meet offers


             //find referees that meet our qualification needs, add them to new list
             bool refereeAlredyAdded = false; //stops adding the same referee multiple times

             foreach (REFEREE referee in tempRefs){
                 foreach (OFFER off in offers){
                     foreach (OFFERQUAL oQual in off.OFFERQUALs){
                         foreach (USERQUAL singleReferee in referee.USERQUALs){
                             //go through each referee, and compare their qualifications to the match/offer
                             if (!refereeAlredyAdded){
                                 refs.Add(referee);
                                 refereeAlredyAdded = true;
                             }
                         }
                     }
                 }
                 refereeAlredyAdded = false;
             }




             /* 

             List<REFEREE> referees = db.REFEREEs.Include(r => r.AspNetUser).Include(m => m.USERQUALs.Select(y => y.QUALIFICATION)).Include(m => m.TIMEOFF).Include(m => m.WEEKLYAVAILABILITY).ToList(); //possibly add where sport = x tournament =y
             List<OFFER> offers = db.OFFERs.Where(m => m.status == 0).Include(m => m.TYPEs).Include(m => m.MATCH).Include(m => m.OFFERQUALs.Select(y => y.QUALIFICATION)).ToList();
             List<REFEREE> refereesWeCanUse = new List<REFEREE>();


             foreach (REFEREE reff in referees)
             {
                 foreach (OFFER off in offers){
                     if(reff.USERQUALs.Intersect(off.OFFERQUALs)){

                     }
                 }
                 if(reff.)
                 foreach (USERQUAL qual in reff.USERQUALs)
                 {
                     sel.Add(new SelectListItem { Text = qual.QUALIFICATION.name, Value = qual.qID.ToString() });
                 }


             }
             * */

            //junk view return
            OFFER offer = db.OFFERs.Find(1);
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
