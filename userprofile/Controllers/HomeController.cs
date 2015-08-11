using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;

namespace userprofile.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

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
            var eventList = GetEvent();

            var rows = eventList.ToArray();
            return Json(rows, JsonRequestBehavior.AllowGet);
        }

        private List<Event> GetEvent()
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

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }
        [HttpPost]
        public ActionResult AddEvent(Event newE)
        {
            var i = Request["id"];
            return null;
        }
    }
}