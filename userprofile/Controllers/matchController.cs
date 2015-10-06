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

        // default is upcoming matches
        public ActionResult Index()
        {
            DateTime thisDay = DateTime.Today;

            var matches = db.MATCHes.Include(m => m.LOCATION).Include(m => m.TEAM).Include(m => m.TEAM1).Include(m => m.TOURNAMENT).Where(match => match.matchDate > thisDay);
            return View(matches.ToList());
        }

        public ActionResult History()
        {
            DateTime thisDay = DateTime.Today;

            var matches = db.MATCHes.Include(m => m.LOCATION).Include(m => m.TEAM).Include(m => m.TEAM1).Include(m => m.TOURNAMENT).Where(match => match.matchDate < thisDay);
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
            var offers = db.OFFERs.Where(o => o.matchId == id).ToList();
           
            if (match == null)
            {
                return HttpNotFound();
            }
            var combined = new Tuple<MATCH, List<OFFER>>(match, offers) { };

            return View(combined);
        }

        // GET: /match/Create
        public ActionResult Create()
        {
            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name");
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "sport");
            ViewBag.qualification = new SelectList(db.QUALIFICATIONS, "qualificationId", "name");
            ViewBag.types = new SelectList(db.TYPEs, "name", "name");
            return View();
        }

        // POST: /match/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MmatchViewModel matchVM)
        {
            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name");
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "sport");
            ViewBag.qualification = new SelectList(db.QUALIFICATIONS, "qualificationId", "name");
            ViewBag.types = new SelectList(db.TYPEs, "name", "name");
            var param1 = this.Request.QueryString["offers"];
            // matchVM.createdMatch.matchDate = (System.DateTime)matchVM.createdMatch.matchDate;
            if (ModelState.IsValid)
            { 
                MATCH newMacth = matchVM.createdMatch;
                string matchsport=db.TOURNAMENTs.Find(newMacth.tournamentId).sport;
                foreach (var newoffer in matchVM.offers) {
                    OFFER nOffer = new OFFER();
                    nOffer.sport = matchsport;
                    nOffer.typeOfOffer = newoffer.type;
                    nOffer.OFFERQUALs.Add(new OFFERQUAL() {qualificationId=newoffer.q,qualLevel=newoffer.level});
                    nOffer.dateOfOffer = System.DateTime.Now;
                    nOffer.status = 4;
                    nOffer.refId = null;
                    //                    Status
                    //0 Deactivated/Cancelled
                    //1 Referee Assigned & Pending
                    //2 Referee Assigned & Accepted
                    //3 Referee Assigned & Declined
                    //4 Algorithm to Assign
                    //5 Unassigned

                    newMacth.OFFERs.Add(nOffer);
                }
               
             
                db.MATCHes.Add(newMacth);

                //add exception catch here to skip return for date check
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name", matchVM.createdMatch.locationId);
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name", matchVM.createdMatch.teamAId);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name", matchVM.createdMatch.teamBId);
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
                var typelist = new List<SelectListItem>();
                foreach (TYPE ty in db.TYPEs) {
                    var tyele = new SelectListItem();
                    tyele.Text = ty.name;
                    tyele.Value = ty.name;
                    typelist.Add(tyele);                    
                }
               // typelist.Select();
                //var refereesLists = new SelectList(, db.REFEREEs.ToList().First(r => r.refID ==2));
                ViewBag.referees = refereesList;
                ViewBag.types = typelist;

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
                string offerType = "offer[" + i + "][typename]";
                string indexofrefid = "offer[" + i + "][refID]";
               var offerID= Convert.ToInt32(Request[indexofid]);
                int mid=Convert.ToInt32(Request["mID"]);
                int status=Convert.ToInt32(Request["offer[" + i + "][status]"]);
                string typename = Request[offerType];
              var stringrefID=  Request[indexofrefid];
               
              if (stringrefID != "") {
                   var refID= Convert.ToInt32(Request[indexofrefid]) ;
                   if (offerID != 0)
                   {
                       OFFER of = db.OFFERs.First(o => o.offerId == offerID);
                       
                       if (of.refId != refID||of.typeOfOffer!=typename)
                       {
                           of.typeOfOffer = typename;
                           of.refId = refID;
                           if (of.refId != refID) { 
                           of.status = 3;
                           }
                           
                           db.Entry(of).State = EntityState.Modified;

                       }
                   }
                   else {
                       var newof = new OFFER();
                       newof.status =3;
                       newof.refId = refID;
                       newof.typeOfOffer = typename;
                       newof.dateOfOffer = System.DateTime.Now;
                       newof.matchId = mid;
                       newof.sport = "soccer";
                       db.OFFERs.Add(newof);
                   }
              }
              else if (status == 4) {
                  var newof = new OFFER();
                  newof.status = 4;
                  newof.typeOfOffer = typename;
                  newof.refId = 63699895;
                  newof.dateOfOffer = System.DateTime.Now;
                  newof.matchId = mid;
                  newof.sport = db.MATCHes.Find(mid).TOURNAMENT.sport;
                  db.OFFERs.Add(newof);

              }
                i++;
          }
            db.SaveChanges();
            return RedirectToAction("Index");

        }


        [HttpGet]
        public ActionResult manageOffer2(int? id)
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
                    sli.Text = re.AspNetUser.lastName + " " + re.AspNetUser.firstName;
                    sli.Value = re.refId.ToString();
                    refereesList.Add(sli);

                }
                var typelist = new List<SelectListItem>();
                foreach (TYPE ty in db.TYPEs)
                {
                    var tyele = new SelectListItem();
                    tyele.Text = ty.name;
                    tyele.Value = ty.name;
                    typelist.Add(tyele);
                }
                // typelist.Select();
                //var refereesLists = new SelectList(, db.REFEREEs.ToList().First(r => r.refID ==2));
                ViewBag.referees = refereesList;
                ViewBag.types = typelist;
                var qlist = new List<SelectListItem>();
                foreach (QUALIFICATION q in db.QUALIFICATIONS) {
                    var qitem = new SelectListItem();
                    qitem.Text = q.name;
                    qitem.Value = q.qualificationId.ToString();
                    qlist.Add(qitem);
                }
                ViewBag.qualification = qlist;
                refereesList.Select(x => refereesList.First(r => r.Value == "2"));
                MoffersViewModels offers = new MoffersViewModels(theMatch);
                return View(offers);

            }
            else
            {

                return RedirectToAction("index", "match");
            }
        }
        [HttpPost]
        public ActionResult manageOffer2()
        {
            var i = 0;
            while (i < 9)
            {
                string indexofid = "offer[" + i + "][ID]";
                string offerType = "offer[" + i + "][typename]";
                string indexofrefid = "offer[" + i + "][refID]";
                string offerqualification = "offer[" + i + "][qualification]";
                string qualificationlevel="offer[" + i + "][[qlevel]";
                var offerID = Convert.ToInt32(Request[indexofid]);
                int mid = Convert.ToInt32(Request["mID"]);
                int status = Convert.ToInt32(Request["offer[" + i + "][status]"]);
                string typename = Request[offerType];
                var stringrefID = Request[indexofrefid];
                var stringq=Request[offerqualification];
                if (stringq == "" || Request[qualificationlevel] == "")
                {

                }
                else
                {
                    var offerQ = Convert.ToInt32(Request[offerqualification]);
                    var qLevel = Convert.ToInt32(Request[qualificationlevel]);


                    if (stringrefID != "")
                    {
                        var refID = Convert.ToInt32(Request[indexofrefid]);
                        if (offerID != 0)
                        {
                            OFFER of = db.OFFERs.First(o => o.offerId == offerID);

                            if (of.refId != refID || of.typeOfOffer != typename)
                            {
                                of.typeOfOffer = typename;
                                of.refId = refID;
                                of.OFFERQUALs.Clear();
                                of.OFFERQUALs.Add(new OFFERQUAL() { qualificationId = offerQ, qualLevel = qLevel });
                                if (of.refId != refID)
                                {
                                    of.status = 3;
                                }

                                db.Entry(of).State = EntityState.Modified;

                            }
                        }
                        else
                        {
                            var newof = new OFFER();
                            newof.status = 3;
                            newof.refId = refID;
                            newof.typeOfOffer = typename;
                            newof.dateOfOffer = System.DateTime.Now;
                            newof.matchId = mid;
                            newof.sport = "soccer";
                            newof.OFFERQUALs.Add(new OFFERQUAL() { qualificationId = offerQ, qualLevel = qLevel });
                            db.OFFERs.Add(newof);
                        }
                    }
                    else if (status == 4)
                    {
                        var newof = new OFFER();
                        newof.status = 4;
                        newof.typeOfOffer = typename;
                        newof.refId = 63699895;
                        newof.dateOfOffer = System.DateTime.Now;
                        newof.matchId = mid;
                        newof.OFFERQUALs.Add(new OFFERQUAL() { qualificationId = offerQ, qualLevel = qLevel });
                        newof.sport = db.MATCHes.Find(mid).TOURNAMENT.sport;
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
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name", match.teamAId);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name", match.teamBId);
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
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name", match.teamAId);
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name", match.teamBId);
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
