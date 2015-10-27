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
    public class MatchController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // default is upcoming matches
        public ActionResult Index()
        {   


            DateTime thisDay = DateTime.Today;

            var matches = db.MATCHes.Include(m => m.LOCATION).Include(m => m.TEAM).Include(m => m.TEAM1).Include(m => m.TOURNAMENT).Where(match => match.matchDate > thisDay).Where(match => match.status >0);
            List<MATCH> matchlist=matches.ToList();
            List<MATCH> matchintournament=new List<MATCH>();

            if (Session["tournamentID"] != null)
            {
                string tid = Session["tournamentID"] as string;
                int tournamentID =Int32.Parse(tid);
                foreach (var match in matchlist)
                {
                    if (match.tournamentId == tournamentID)
                    {
                        matchintournament.Add(match);

                    }
                }

            }
            else {
                matchintournament = matchlist;
            
            }
            return View(matchintournament);
        }
   
        public List<REFEREE> getAvailableRefereesForOffer(int oID)
        {
            OFFER offer = db.OFFERs.Find(oID);
            List<REFEREE> availableReferees = new List<REFEREE>();
            List<KeyValuePair<int, int>> offerQuals = new List<KeyValuePair<int, int>>();
			DateTime matchDate = offer.MATCH.matchDate;

            if (offer.OFFERQUALs==null) {

                foreach (var i in db.REFEREEs)
                {
                    if (containsOneOff(matchDate, i.refId))
                    {
						if (checkOneOff(matchDate, i.refId))
                        {
                            availableReferees.Add(i);
                        }
                    }
                    else
                    {
						if (checkWeeklyAvailabilityForMatch(getWeeklyAvailabilityForDay(matchDate, i.refId), matchDate))
                        {
                            availableReferees.Add(i);
                        }
                    }
                }
            }
            else {
                foreach (var i in offer.OFFERQUALs)
                {
                    offerQuals.Add(new KeyValuePair<int, int>(i.qualificationId, i.qualLevel));
                }
                foreach (var i in db.REFEREEs)
                {
                    bool refHasQualification = true;
                    foreach (var j in offerQuals)
                    {
                        try
                        {
                            db.USERQUALs.Find(j.Key, i.refId);
                        }
                        catch (SystemException a)
                        {
                            refHasQualification = false;
                            break;
                        }
                    }
                    if (refHasQualification)
                    {
                        //check if has a free slot to ref on that day (is currently reffing < maxGames)

						if (containsOneOff(matchDate, i.refId))
                        {
							if (checkOneOff(matchDate, i.refId))
                            {
                                availableReferees.Add(i);
                            }
                        }
                        else
                        {
							if (checkWeeklyAvailabilityForMatch(getWeeklyAvailabilityForDay(matchDate, i.refId), matchDate))
                            {
                                availableReferees.Add(i);
                            }
                        }
                    }
                }
            }
      
            return availableReferees;
        }
        bool containsOneOff(DateTime matchDateTime, int rID)
        { //REDO
            try
            {
                var temp = db.OneOffAVAILABILITies.Find(rID, matchDateTime.Date); //WRITE PRIMARY KEY FOR REFAVAILABILITY
                if (temp == null)
                    return false;
                else
                    return true;
            }
            catch
            {
                return false;
            }
        }
        int getWeeklyAvailabilityForDay(DateTime dt, int rID)
        {
            try
            {
                switch (dt.DayOfWeek)
                {
                    case DayOfWeek.Sunday:
                        return db.WEEKLYAVAILABILITies.Find(rID).sunday;
                    case DayOfWeek.Monday:
                        return db.WEEKLYAVAILABILITies.Find(rID).monday;
                    case DayOfWeek.Tuesday:
                        return db.WEEKLYAVAILABILITies.Find(rID).tuesday;
                    case DayOfWeek.Wednesday:
                        return db.WEEKLYAVAILABILITies.Find(rID).wednesday;
                    case DayOfWeek.Thursday:
                        return db.WEEKLYAVAILABILITies.Find(rID).thursday;
                    case DayOfWeek.Friday:
                        return db.WEEKLYAVAILABILITies.Find(rID).friday;
                    case DayOfWeek.Saturday:
                        return db.WEEKLYAVAILABILITies.Find(rID).saturday;
                    default:
                        //error
                        return 0;
                }
            }
            catch
            {
                return 0;
            }
        }
        bool checkOneOff(DateTime matchDateTime, int rID)
        {
            try
            {
                var temp = db.OneOffAVAILABILITies.Find(rID, matchDateTime.Date);
                if (temp.timeOnOrOff == true)
                    return true;
                else
                    return false;
            }
            catch (SystemException a)
            {
                return false;
            }

        }
        bool checkWeeklyAvailabilityForMatch(int weeklyAvailability, DateTime matchDateTime)
        {
            if (weeklyAvailability == 0)
            {
                return false;
            }
            if (weeklyAvailability >= 8)
            {
                if (matchDateTime.TimeOfDay < new TimeSpan(6, 0, 0))
                {
                    return true;
                }
                weeklyAvailability -= 8;
            }
            else if (matchDateTime.TimeOfDay < new TimeSpan(6, 0, 0))
            {
                return false;
            }
            if (weeklyAvailability >= 4)
            {
                if (matchDateTime.TimeOfDay < new TimeSpan(12, 0, 0))
                {
                    return true;
                }
                weeklyAvailability -= 4;
            }
            else if (matchDateTime.TimeOfDay < new TimeSpan(12, 0, 0))
            {
                return false;
            }
            if (weeklyAvailability >= 2)
            {
                if (matchDateTime.TimeOfDay < new TimeSpan(18, 0, 0))
                {
                    return true;
                }
                weeklyAvailability -= 2;
            }
            else if (matchDateTime.TimeOfDay < new TimeSpan(18, 0, 0))
            {
                return false;
            }
            if (weeklyAvailability >= 1)
            {
                if (matchDateTime.TimeOfDay < new TimeSpan(24, 0, 0))
                {
                    return true;
                }
                weeklyAvailability -= 1;
            }
            else if (matchDateTime.TimeOfDay < new TimeSpan(24, 0, 0))
            {
                return false;
            }
            throw new SystemException();
        }
        public ActionResult History()
        {
            DateTime thisDay = DateTime.Today;

            var matches = db.MATCHes.Include(m => m.LOCATION).Include(m => m.TEAM).Include(m => m.TEAM1).Include(m => m.TOURNAMENT).Where(match => match.matchDate < thisDay).Where(match => match.status > 0);
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
		[HttpPost]
		public ActionResult getTeamsFromTournament(int? tournamentId) {
			var teamsInTournament = db.TEAMINS.Where(t => t.tournament == tournamentId).ToList();
			List<TEAM> teams = new List<TEAM>();
			foreach (var i in teamsInTournament) {
				teams.Add(i.TEAM);
			}
			SelectList teamViewBag = new SelectList(teams, "teamId", "name");
			return Json(teamViewBag);
		}

        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Create()
        {
            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "name");
            ViewBag.qualification = new SelectList(db.QUALIFICATIONS, "qualificationId", "name");
            ViewBag.types = new SelectList(db.TYPEs, "name", "name");
            return View();
        }

        // POST: /match/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Create(MmatchViewModel matchVM)
        {
            if (User.IsInRole("Organizer")) { 
                TOURNAMENT t = db.TOURNAMENTs.First(tt=>tt.AspNetUser.UserName==User.Identity.Name);
                matchVM.createdMatch.tournamentId = t.tournamentId;
                matchVM.createdMatch.TOURNAMENT = t;
            }
            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name");
            ViewBag.teamaID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.teambID = new SelectList(db.TEAMs, "teamId", "name");
            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "name");
            ViewBag.qualification = new SelectList(db.QUALIFICATIONS, "qualificationId", "name");
            ViewBag.types = new SelectList(db.TYPEs, "name", "name");
            var param1 = this.Request.QueryString["offers"];
            // matchVM.createdMatch.matchDate = (System.DateTime)matchVM.createdMatch.matchDate;
            if (ModelState.IsValid)
            { 

                MATCH newMatch = matchVM.createdMatch;
                newMatch.status = 1;
                if (matchVM.offers != null && matchVM.offers.Length > 0)
                { 
                    foreach (var newoffer in matchVM.offers) {
                        OFFER nOffer = new OFFER();
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

                        newMatch.OFFERs.Add(nOffer);
                    }
                }

                db.MATCHes.Add(newMatch);

                //add exception catch here to skip return for date check
                db.SaveChanges();
                return RedirectToAction("Index");
            }


            ViewBag.location = new SelectList(db.LOCATIONs, "locationId", "name", matchVM.createdMatch.locationId);

            ViewBag.tournament = new SelectList(db.TOURNAMENTs, "tournamentId", "name");
            ViewBag.qualification = new SelectList(db.QUALIFICATIONS, "qualificationId", "name");
            return View(matchVM);
        }

  
     

        [Authorize(Roles = "Admin,Organizer")]
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
                    sli.Text = re.AspNetUser.firstName + " " + re.AspNetUser.lastName;
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
					if (q.status > 0) {
						var qitem = new SelectListItem();
						qitem.Text = q.name;
						qitem.Value = q.qualificationId.ToString();
						qlist.Add(qitem);
					}
                }
                ViewBag.qualification = qlist;
                refereesList.Select(x => refereesList.First(r => r.Value == "2"));
                MoffersViewModels offers = new MoffersViewModels(theMatch);
                int i = 0;
                foreach (var aoffer in offers.offers) {
                    if (aoffer.status != 1 && aoffer.status != 0 && aoffer.status != 6)
                    {
                        var screenedList = new List<SelectListItem>();
                        
                    foreach(REFEREE re in getAvailableRefereesForOffer(aoffer.offerId)){
                        var sli = new SelectListItem();
                        sli.Text = re.AspNetUser.firstName + " " + re.AspNetUser.lastName;
                        sli.Value = re.refId.ToString();
                        screenedList.Add(sli);
                    }
                    offers.screeneddropdown.Add(screenedList);
                    }
                }
                return View(offers);

            }
            else
            {

                return RedirectToAction("index", "match");
            }
        }

        [Authorize(Roles = "Admin,Organizer")]
        [HttpPost]
        public ActionResult manageOffer()
        {
            var i = 0;
            while (i < 9)
            {
                string indexofid = "offer[" + i + "][ID]";
                string offerType = "offer[" + i + "][typename]";
                string indexofrefid = "offer[" + i + "][refID]";
                string offerqualification = "offer[" + i + "][qualification]";
                string qualificationlevel = "qlevel[" + i + "]";
                  int mid = Convert.ToInt32(Request["mID"]);
                 int status = Convert.ToInt32(Request["offer[" + i + "][status]"]);
                 string typename = Request[offerType];
                 var stringrefID = Request[indexofrefid];
                var stringq=Request[offerqualification];
                 var offerID=0;
                 //var qlevel = Request["qlevel[8]"];
                 OFFER of=new OFFER() ;
                 switch (status) { 
                     case 1:
                          offerID = Convert.ToInt32(Request[indexofid]);
                         of = db.OFFERs.Find(offerID);
                         if(of.status!=status){
                          db.Entry(of).State = EntityState.Modified;
                         }
                         break;
                     case 0:
                         offerID = Convert.ToInt32(Request[indexofid]);
                         of = db.OFFERs.Find(offerID);
                         of.status = 0;
                         db.Entry(of).State = EntityState.Modified;
                         break;
                     case 6:
                         if (typename == "")
                         {
                             //not valid offer
                         }
                         else {
                             of.typeOfOffer = typename;
                             of.status=4;
                             of.dateOfOffer = System.DateTime.Today;
                             of.matchId = mid;
                             if (stringq != "")
                             {
                                 if (Request[qualificationlevel] != "")
                                 {
                                    string ql=Request[qualificationlevel];
                                     of.OFFERQUALs.Add(new OFFERQUAL { qualLevel = Convert.ToInt32(Request[qualificationlevel]), qualificationId = Convert.ToInt32(Request[offerqualification]) });

                                 }
                                 else
                                 {
                                     of.OFFERQUALs.Add(new OFFERQUAL { qualLevel = 1, qualificationId = Convert.ToInt32(Request[offerqualification]) });
                                 }
                             }
                             else { 
                             }
                             db.OFFERs.Add(of);

                         }
                         break;
                     default:
                          offerID = Convert.ToInt32(Request[indexofid]);
                          of = db.OFFERs.Find(offerID);
                          if (of.status==status&&status==5) { 
                          //nothing Changed
                          }
                          else if (of.status != status && status == 5) {
                              of.status = 5;
                              db.Entry(of).State = EntityState.Modified;
                          }
                          else if (Request[indexofrefid] == ""&&of.refId!=null) {

                              of.status = 4;
                              db.Entry(of).State = EntityState.Modified;
                          }
                          else if (Request[indexofrefid] == "" && of.refId == null&&of.status!=4) {
                              of.status = 4;
                              db.Entry(of).State = EntityState.Modified;

                          }
                          else if (Request[indexofrefid] == "" && of.refId == null && of.status == 4)
                          {
                              //do nothing

                          }
                          else if (of.refId != Convert.ToInt32(Request[indexofrefid])) { 
                          //set to pending
                              of.status = 3;
                              of.refId = Convert.ToInt32(Request[indexofrefid]);
                              db.Entry(of).State = EntityState.Modified;
                          }
                       
                          break;

                 }
                 


///////////////////////////////////////////////////////////////////////////////
                //string indexofid = "offer[" + i + "][ID]";
                //string offerType = "offer[" + i + "][typename]";
                //string indexofrefid = "offer[" + i + "][refID]";
                //string offerqualification = "offer[" + i + "][qualification]";
                //string qualificationlevel="offer[" + i + "][[qlevel]";
                //var offerID = Convert.ToInt32(Request[indexofid]);
                //int mid = Convert.ToInt32(Request["mID"]);
                //int status = Convert.ToInt32(Request["offer[" + i + "][status]"]);
                //string typename = Request[offerType];
                //var stringrefID = Request[indexofrefid];
                //var stringq=Request[offerqualification];
                //if (typename=="")
                //{

                //}
                //else
                //{
                //    var offerQ = Convert.ToInt32(Request[offerqualification]);
                //    var qLevel = Convert.ToInt32(Request[qualificationlevel]);


                //    if (stringrefID != "")
                //    {
                //        var refID = Convert.ToInt32(Request[indexofrefid]);
                //        if (offerID != 0)
                //        {
                //            OFFER of = db.OFFERs.First(o => o.offerId == offerID);

                //            if (of.refId != refID || of.typeOfOffer != typename)
                //            {
                //                of.typeOfOffer = typename;
                //                of.refId = refID;
                //                of.OFFERQUALs.Clear();
                //                of.OFFERQUALs.Add(new OFFERQUAL() { qualificationId = offerQ, qualLevel = qLevel });
                //                if (of.refId != refID)
                //                {
                //                    of.status = 3;
                //                }

                //                db.Entry(of).State = EntityState.Modified;

                //            }
                //        }
                //        else
                //        {
                //            var newof = new OFFER();
                //            newof.status = 3;
                //            newof.refId = refID;
                //            newof.typeOfOffer = typename;
                //            newof.dateOfOffer = System.DateTime.Now;
                //            newof.matchId = mid;
                //            newof.OFFERQUALs.Add(new OFFERQUAL() { qualificationId = offerQ, qualLevel = qLevel });
                //            db.OFFERs.Add(newof);
                //        }
                //    }
                //    else if (status == 4)
                //    {
                //        var newof = new OFFER();
                //        newof.status = 4;
                //        newof.typeOfOffer = typename;
                //        newof.refId = 63699895;
                //        newof.dateOfOffer = System.DateTime.Now;
                //        newof.matchId = mid;
                //        newof.OFFERQUALs.Add(new OFFERQUAL() { qualificationId = offerQ, qualLevel = qLevel });
                //        db.OFFERs.Add(newof);

                //    }

                //}
                /////////////////////////////////////////////////////
                i++;
            }
            db.SaveChanges();
            return RedirectToAction("Index");

        }

        [Authorize(Roles = "Admin,Organizer")]
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
            ViewBag.tournamentId = new SelectList(db.TOURNAMENTs, "tournamentId", "name", match.tournamentId);
            return View(match);
        }

        // POST: /match/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Edit([Bind(Include = "matchId,matchDate,locationId,teamaId,teambId,teamAScore,teamBScore,status,tournamentId,matchLength,halfTimeDuration,countsToDraw")] MATCH match)
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
            ViewBag.tournamentId = new SelectList(db.TOURNAMENTs, "tournamentId", "name", match.tournamentId);
            return View(match);
        }

        [Authorize(Roles = "Admin,Organizer")]
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
            return View(match);
        }

        [Authorize(Roles = "Admin,Organizer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MATCH match = db.MATCHes.Find(id);

            var offersToHide = db.OFFERs.Where(offer => offer.matchId == id);

            foreach (OFFER offers in offersToHide)
            {
                offers.status = 0;
                db.Entry(offers).State = EntityState.Modified;
            }

            match.status= 0;
            db.Entry(match).State = EntityState.Modified;

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
