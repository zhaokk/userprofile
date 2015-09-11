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
    public class matchController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: /match/
        public ActionResult Index()
        {
            var matches = db.MATCHes.Include(m => m.LOCATION).Include(m => m.TEAM).Include(m => m.TEAM1).Include(m => m.TOURNAMENT);
            return View(matches.ToList());
        }

        // GET: /match/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATCH match = db.MATCHes.Find(id);
           
            if (match == null)
            {
                return HttpNotFound();
            }
            return View(match);
        }

        // GET: /match/Create
        public ActionResult Create()
        {
            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name");
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "sport");
            ViewBag.qualification = new SelectList(db.QUALIFICATIONS, "qualificationId", "name");
            return View();
        }

        // POST: /match/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( matchViewModel matchVM)
        {


            matchVM.createdMatch.matchDate = (System.DateTime)matchVM.createdMatch.matchDate;
            if (ModelState.IsValid)
            {
                
                MATCH newMacth = matchVM.createdMatch;
                OFFER offer1 = new OFFER();
                OFFER offer2 = new OFFER();
                OFFER offer3 = new OFFER();
                offer1.status = offer2.status = offer3.status = 4;
                offer1.SPORT1 = offer2.SPORT1 = offer3.SPORT1 = db.SPORTs.Find("Soccer");
                switch (matchVM.offernum) { 
                    case 1:
                        
                        //offer1.QUALIFICATIONS.Add(db.QUALIFICATIONS.Find(matchVM.q1));
                        newMacth.OFFERs.Add(offer1);
                       break;
                    case 2:
                        
                       // offer1.QUALIFICATIONS.Add(db.QUALIFICATIONS.Find(matchVM.q1));
                        
                        //offer2.QUALIFICATIONS.Add(db.QUALIFICATIONS.Find(matchVM.q1));
                        newMacth.OFFERs.Add(offer1);
                        newMacth.OFFERs.Add(offer2);

                        break;
                    case 3:
                        
                        //offer1.QUALIFICATIONS.Add(db.QUALIFICATIONS.Find(matchVM.q1));
                        
                        //offer2.QUALIFICATIONS.Add(db.QUALIFICATIONS.Find(matchVM.q1));
                        
                        //offer3.QUALIFICATIONS.Add(db.QUALIFICATIONS.Find(matchVM.q1));
                        newMacth.OFFERs.Add(offer1);
                        newMacth.OFFERs.Add(offer2);
                        newMacth.OFFERs.Add(offer3);
                        break;
                    default:
                        break;
                }
                db.MATCHes.Add(newMacth);

                //add exception catch here to skip return for date check
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name", matchVM.createdMatch.locationId);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name", matchVM.createdMatch.teamaId);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name", matchVM.createdMatch.teambId);
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "sport", matchVM.createdMatch.tournamentId);
            ViewBag.qualification = new SelectList(db.QUALIFICATIONS, "qualificationId", "name");
            return View(matchVM);
        }
        [HttpGet]
        public ActionResult manageOffer(int? id)
        {
            if (id != null)
            {
                MATCH theMatch = db.MATCHes.First(m => m.matchId == id);
                var refereesList = new List<SelectListItem>()
                   {





                   };
            
                foreach (REFEREE re in db.REFEREEs)
                {
                    var sli = new SelectListItem();
                    sli.Text = re.AspNetUser.lastName +" "+ re.AspNetUser.firstName;
                    sli.Value = re.refId.ToString();
                    refereesList.Add(sli);

                }
                //var refereesLists = new SelectList(, db.REFEREEs.ToList().First(r => r.refID ==2));
                ViewBag.referees = refereesList;
                refereesList.Select(x=>refereesList.First(r => r.Value == "2"));
                offersViewModels offers = new offersViewModels(theMatch);
                return View(offers);

            }
            else {

                return RedirectToAction("index", "match");
            }
        }
        [HttpPost]
        public ActionResult manageOffer() {
          var i=0;
            while(i<3){
                string indexofid = "offer[" + i + "][ID]";
                string indexofrefid = "offer[" + i + "][refID]";
               var offerID= Convert.ToInt32(Request[indexofid]);
                int mid=Convert.ToInt32(Request["mID"]);
              var stringrefID=  Request[indexofrefid];
              if (stringrefID != "") {
                   var refID= Convert.ToInt32(Request[indexofrefid]) ;
                   if (offerID != 0)
                   {
                       OFFER of = db.OFFERs.First(o => o.offerId == offerID);
                       if (of.refId != refID)
                       {
                           of.refId = refID;
                           of.status = 3;
                           db.Entry(of).State = EntityState.Modified;

                       }
                   }
                   else {
                       var newof = new OFFER();
                       newof.status =3;
                       newof.refId = refID;
                       newof.dateOfOffer = System.DateTime.Now;
                       newof.matchId = mid;
                       newof.sport = "soccer";
                       db.OFFERs.Add(newof);
                   }
              }
                i++;
          }
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        // GET: /match/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATCH match = db.MATCHes.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            ViewBag.locationId = new SelectList(db.LOCATIONs, "locationId", "name", match.locationId);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name", match.teamaId);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name", match.teambId);
            ViewBag.tournamentId = new SelectList(db.TOURNAMENTs, "tournamentId", "sport", match.tournamentId);
            return View(match);
        }

        // POST: /match/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "matchId,matchDate,locationId,teamaId,teambId,teamAScore,teamBScore,winnerId,tournamentId,matchLength")] MATCH match)
        {
            if (ModelState.IsValid)
            {
                db.Entry(match).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.locationId = new SelectList(db.LOCATIONs, "locationId", "name", match.locationId);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name", match.teamaId);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name", match.teambId);
            ViewBag.tournamentId = new SelectList(db.TOURNAMENTs, "tournamentId", "sport", match.tournamentId);
            return View(match);
        }

        // GET: /match/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MATCH match = db.MATCHes.Find(id);
            if (match == null)
            {
                return HttpNotFound();
            }
            if (match.OFFERs.Count() != 0) { 
             ViewBag.error = "this match cant be delete because there are related offers";
             return View(match);
            }
            return View(match);
        }

        // POST: /match/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MATCH match = db.MATCHes.Find(id);
            if (match.OFFERs.Count() == 0)
            {
                db.MATCHes.Remove(match);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.error = "this match cant be delete because there are related offers";
                return View(match);
            }
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
