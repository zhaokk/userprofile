using System;
using System.Collections.Generic;
using userprofile.Models;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace userprofile.Controllers
{
    /// <summary>
    /// Handles all notification related things
    /// </summary>
    public class NotificationsController : Controller
    {

        Raoconnection db = new Raoconnection();

        /// <summary>
        /// Dummy index
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "")]
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Returns anything that needs attention:
        /// matches that have no offers
        /// offers that have no referees
        /// players that have requested to join a team
        /// 
        /// </summary>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "Admin,Organizer,Referee")]
        public ActionResult Display()
        {

            //referee things

            List<OFFER> refereeOffers = new List<OFFER>();
            List<MATCH> refereeMatches = new List<MATCH>();


            if (User.IsInRole("Admin"))
            {
                return View(getAdminNotifications());
            }
            else if (User.IsInRole("Organizer"))
            {
                var thisUser = db.AspNetUsers.Where(user => user.UserName == User.Identity.Name).FirstOrDefault();
                return View(getOrganizerNotifications(thisUser));
            }
            if (User.IsInRole("Referee"))
            {
                var thisUser = db.AspNetUsers.Where(user => user.UserName == User.Identity.Name).FirstOrDefault();
                refereeOffers = getRefereeOffers(thisUser.REFEREEs.ElementAt(0).refId);
                refereeMatches = getRefereeMatches(thisUser.REFEREEs.ElementAt(0).refId);

                var combined = new Tuple<List<OFFER>, List<MATCH>, List<PLAYER>>(refereeOffers, refereeMatches, new List<PLAYER>()) { };

                return View(combined);


            }

            //return nothing
            var combinedDud = new Tuple<List<OFFER>, List<MATCH>, List<PLAYER>>(new List<OFFER>(), new List<MATCH>(), new List<PLAYER>()) { };

            return View(combinedDud);
        }

        private List<OFFER> getRefereeOffers(int id)
        {
            List<OFFER> refereeOffers = new List<OFFER>();

            refereeOffers = db.OFFERs.Where(referee => referee.refId == id).Where(referee => referee.status == 3).ToList();

            return refereeOffers;
        }


        private List<MATCH> getRefereeMatches(int id)
        {
            DateTime today = DateTime.Now;

            List<MATCH> refereeMatches = new List<MATCH>();
            var tempReferee = db.REFEREEs.Find(id);


            foreach (var offer in tempReferee.OFFERs)
            {
                if (offer.status == 1 && offer.MATCH.matchDate < today)
                {
                    refereeMatches.Add(offer.MATCH);
                }
            }

            return refereeMatches;
        }



        /// <summary>
        /// gets a tuple of all notifications for an organizer.
        /// These notifications are specific for the tournament that the organizer manages
        /// </summary>
        /// <param name="offers">A blank List to put offers that require attention</param>
        /// <param name="matches">A blank List to put matches that require attention</param>
        /// <param name="players">A blank List to put players that have requested to join a team</param>
        /// <param name="thisUser">The username of the currently logged in user</param>
        /// <returns>A tuple that consists of a list of offers, matches and players</returns>
        /// 
        [Authorize(Roles = "Organizer")]
        private Tuple<List<OFFER>, List<MATCH>, List<PLAYER>> getOrganizerNotifications(AspNetUser thisUser)
        {

            List<OFFER> offers = new List<OFFER>();
            List<MATCH> matches = new List<MATCH>();
            List<PLAYER> players = new List<PLAYER>();


            DateTime nextWeek = DateTime.Now;
            DateTime today = DateTime.Now;
            nextWeek = nextWeek.AddDays(7);

            foreach (var tournament in thisUser.TOURNAMENTs1)
            {


                matches.AddRange(db.MATCHes.Include("OFFERs").Where(match => match.OFFERs.Count == 0)
                                                      .Where(match => match.matchDate < nextWeek)
                                                      .Where(match => match.matchDate >= today)
                                                      .Where(match => match.tournamentId == tournament.tournamentId).ToList());

                offers.AddRange(db.OFFERs.Where(offer => offer.REFEREEs.Count == 0)
                                  .Where(offer => offer.status > 0)
                                  .Where(offer => offer.MATCH.matchDate >= today)
                                  .Where(offer => offer.MATCH.tournamentId == tournament.tournamentId).ToList());


                foreach (var play in players)
                {
                    foreach (var team in play.TEAM.TEAMINS)
                    {
                        if (team.tournament == tournament.tournamentId)
                        {
                            players.Add(play);
                        }
                    }
                }
            }

            return new Tuple<List<OFFER>, List<MATCH>, List<PLAYER>>(offers, matches, players);
        }


        /// <summary>
        /// gets a tuple of all notifications for an admin.
        /// These notifications cover all tournaments
        /// </summary>
        /// <param name="offers">A blank List to put offers that require attention</param>
        /// <param name="matches">A blank List to put matches that require attention</param>
        /// <param name="players">A blank List to put players that have requested to join a team</param>
        /// <returns>A tuple that consists of a list of offers, matches and players</returns>
        /// 
        [Authorize(Roles = "Admin")]
        private Tuple<List<OFFER>, List<MATCH>, List<PLAYER>> getAdminNotifications()
        {

            List<OFFER> offers = new List<OFFER>();
            List<MATCH> matches = new List<MATCH>();
            List<PLAYER> players = new List<PLAYER>();

            DateTime nextWeek = DateTime.Now;
            DateTime today = DateTime.Now;
            nextWeek = nextWeek.AddDays(7);

            matches = db.MATCHes.Include("OFFERs").Where(match => match.OFFERs.Count == 0)
                                                  .Where(match => match.matchDate < nextWeek)
                                                  .Where(match => match.matchDate >= today).ToList();

            offers = db.OFFERs.Where(offer => offer.REFEREEs.Count == 0)
                              .Where(offer => offer.status == 4)
                              .Where(offer => offer.MATCH.matchDate >= today).ToList();

            players = db.PLAYERs.Where(player => player.status == 3).ToList();

            return new Tuple<List<OFFER>, List<MATCH>, List<PLAYER>>(offers, matches, players);
        }

    }



}