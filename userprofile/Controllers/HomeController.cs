using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;
using Microsoft.AspNet.Identity;

namespace userprofile.Controllers
{

    public class HomeController : Controller
    {
        public Raoconnection db = new Raoconnection();
        [Authorize]
        public ActionResult Index()
        {
            var userID = User.Identity.GetUserId();
           
            // ICollection<OFFER> offers = db.REFEREEs.FirstOrDefault(r => r.ID == userID).OFFERs;
            if (db.REFEREEs.FirstOrDefault( r => r.userId == userID) != null)
            {
                List<OFFER> alloffers = db.REFEREEs.FirstOrDefault(r => r.userId == userID).OFFERs.ToList();
                
                offerDataViewModel sortedOffer = new offerDataViewModel(alloffers);
                return View(sortedOffer);
            }
            else if (User.IsInRole("Admin")) {
                return RedirectToAction("IndexforAd", "home");

            }
            else if (User.IsInRole("Organizer")) {
                return RedirectToAction("IndexforOrg", "home");
            }
            else if (User.IsInRole("Manager")) {
                return RedirectToAction("IndexforManager","home");
            
            }
            else
            {
                return RedirectToAction("IndexForAnyone", "home");
            }

        }
        public ActionResult IndexforAd()
        {

            admineOfferViewModel aOVM = new admineOfferViewModel(db.OFFERs.ToList());
            return View(aOVM);
        }
        public ActionResult IndexforOrg()
        {
            var userID = User.Identity.GetUserId();
            if (db.TOURNAMENTs.FirstOrDefault(t => t.organizer == userID) == null) {
                ViewBag.Massage = "you are not managing any tournament";
                return View();
            }
         
            else
            {
                OrgViewModels orgVM = new OrgViewModels(db, userID);
                return View(orgVM);
            }
        }
        public ActionResult IndexforManager() {
            var userID = User.Identity.GetUserId();
            if (db.TEAMs.FirstOrDefault(t => t.managerId == userID) == null)
            {
                ViewBag.Massage = "you are not managing any tournamnet";
                return View();
            }
            else {
                managerViewModels mVM = new managerViewModels(db,userID);
                return View(mVM);
            }
           

        
        }
        public ActionResult IndexForAnyone()
        {

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Calendar() {
            ViewBag.Message = "The Calendar";
            return View();
        }
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        public ActionResult GetEvents(DateTime start, DateTime end)
        {
            //var fromDate = ConvertFromUnixTimestamp(start);
            //var toDate = ConvertFromUnixTimestamp(end);

            //Get the Event1
            //You may get from the repository also
            //if (User.IsInRole("Referee"))
            //{
            var eventList = GetEvent();

            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
            // }
            //  return null;
        }

        private List<Event> GetEvent()
        {
            var db = new Raoconnection();
              var userID = User.Identity.GetUserId();
            if (User.Identity.IsAuthenticated&&db.REFEREEs.First(r => r.userId == userID)!=null)
            {
                var i = 0;
              
                TimeSpan time = new TimeSpan(0, 1, 30, 0);
                List<Event> eventList = new List<Event>();

               
             
                List<OFFER> offers = db.REFEREEs.First(r => r.userId == userID).OFFERs.ToList();
                foreach (OFFER offer in offers)
                {

                    Event newEvent = new Event
                    {
                        Id = i,
                        title = "Match:" + i,
                        start = offer.MATCH.matchDate,
                        end = offer.MATCH.matchDate + time,
                        allDay = false
                    };
                    eventList.Add(newEvent);
                    i++;
                }
            //    eventList.Add(new Event() { start= System.DateTime.Now,
            //                                end = System.DateTime.Now.AddDays(1),
            //rendering= "background" });
                List<OneOffAVAILABILITY> oneoffs = db.REFEREEs.First(r => r.userId == userID).OneOffAVAILABILITies.ToList();
               
                foreach (var oneoff in oneoffs){
                    Event newEvent = new Event
                    {
                        Id = i,
                        title =oneoff.description,
                        start =oneoff.startDate,
                        allDay = true,
                        backgroundColor = "rgb(221, 75, 57)" 
                    };
                    eventList.Add(newEvent);

                    i++;
                }

              
                return eventList;



            }
            else
            {

                List<Event> eventList = new List<Event>();

                Event newEvent = new Event
                {
                    Id = 1,
                    title = "Eventt 1",
                    start = DateTime.Now,
                    end = DateTime.Now,
                    allDay = false
                };


                eventList.Add(newEvent);

                newEvent = new Event
                {
                    Id = 2,
                    title = "Event 3",
                    start = DateTime.Now,
                    end = DateTime.Now,
                    allDay = false
                };

                eventList.Add(newEvent);
                
                return eventList;
            }

        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        [HttpPost]
        public Boolean AddEvent(Event newE)
        {
            var db = new Raoconnection();
            var userID = User.Identity.GetUserId();
          //  var refID=db.REFEREEs.First(r=>r.userId==userID).refId;
            OneOffAVAILABILITY of=new OneOffAVAILABILITY(){
               
            refId=1000,
            startDate=(System.DateTime)newE.start,
            timeOnOrOff=false,
            description=newE.title
            };
            if (ModelState.IsValid)
            {
              //  db.OneOffAVAILABILITies.Add(of);
                db.OneOffAVAILABILITies.Add(of);
                db.SaveChanges();
                return true;
            }
            return true;
            
        }
        public ActionResult checkOffer()
        {

            var userID = User.Identity.GetUserId();
            var db = new Raoconnection();
            if (User.IsInRole("Referee"))
            {
                REFEREE refe = db.REFEREEs.First(r => r.userId == userID);

                return View(refe.OFFERs);
            }



            return View();
        }
        [Authorize(Roles = "Referee")]
        public ActionResult acceptOffer(int refeID)
        {



            return View();
        }
        [Authorize(Roles = "Referee")]
        public ActionResult denyOffer(int refeID)
        {

            return View();
        }






    }
}