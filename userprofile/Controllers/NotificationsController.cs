using System;
using System.Collections.Generic;
using userprofile.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace userprofile.Controllers
{
    public class NotificationsController : Controller
    {
        //
        // GET: /Notifications/
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Returns anything that needs attention
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Display()
        {

            //User.Identity.Name
            Raoconnection db = new Raoconnection();

            List<OFFER> offers = new List<OFFER>();
            List<MATCH> matches = new List<MATCH>();
            List<PLAYER> players = new List<PLAYER>();

            DateTime nextWeek = DateTime.Now;
            nextWeek = nextWeek.AddDays(7);


            if (User.IsInRole("Admin"))
            {
                matches = db.MATCHes.Where(match => match.OFFERs.Count == 0).Where(match => match.matchDate < nextWeek).ToList();
                offers = db.OFFERs.Where(offer => offer.REFEREEs.Count == 0).Where(offer => offer.status > 0).ToList();
            }



                matches = db.MATCHes.Where(match => match.OFFERs.Count == 0).Where(match => match.matchDate < nextWeek).ToList();

            var combined = new Tuple<List<OFFER>, List<MATCH>, List<PLAYER>>(offers, matches, players) { };

            return View(combined);
        }
	}
}