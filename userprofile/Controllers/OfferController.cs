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
    public class OfferController : Controller
    {
        //get the database context
        private Raoconnection db = new Raoconnection();




        //
        // GET: /Offer/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Edit(int tournamnet)
        {
            

            //get offers & attached match that have not been assigned
            var offers = db.OFFERs.Where(m => m.status == 1).Include(m => m.MATCH); // date in the future
            List<USERQUAL> qualificationsThatExist = new List<USERQUAL>();

            var qualifications = db.OFFERQUALs;

            var refereesWeWant =  db.REFEREEs.Include(m => m.USERQUALs);
            List<REFEREE> refArray = refereesWeWant.ToList();
            qualificationsThatExist = refArray.USERQUAL.ToList();

           // IEnumerable<REFEREE> refsWeActualyWant = refereesWeWant.Intersect(qualifications);





            var offerQualifications = new OFFERQUAL();

            for (var i = 0; i < offers.Count(); i++){
               // var qualifications = db.OFFERQUALs.Where(m=> m.offerID in offers.id);

            }


            //for()
            //var offerQualifications = db.OFFERQUALs.where

            var matches = db.MATCHes.Include(m => m.LOCATION1).Include(m => m.TEAM).Include(m => m.TEAM1).Include(m => m.TOURNAMENT1);

            if (tournamnet == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATCH match = db.MATCHes.Find(tournamnet);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.location = new SelectList(db.LOCATIONs, "lID", "name", match.location);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamID", "name", match.teamaID);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamID", "name", match.teambID);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tID", "sport", match.tournament);
            return View(match);
        }


	}
}