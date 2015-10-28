/*
 * This code was written by David King (d@kingzown.com) for the express purpose of University of Wollongong's subject CSCI321 Major Project for the group CSCI321JG1 2015.
 * Do not use or reference this code without David King's (d@kingzown.com) written permission.
 * This code also includes references to work done by Kang Zhou, Glen Wiltshire, Chenhao Wei
 * 
 * If this code is used please include the following statement at the top of your program, and append to the credits:
 * "This code is written by or contains reference to work done by David King, Kang Zhou, Glen Wiltshire and Chenhao Wei"
 * 
 */

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;

namespace userprofile.Controllers {
	public class AlgorithmController : Controller {

		/// <summary>
		///Runs algorithm without any dates.  Used in previous version
		/// </summary>
		/// <returns></returns>
		public ActionResult showResults() {
			AssignReferees();
			modelResult.sortResult();
			return View(modelResult);
		}

		/// <summary>
		/// Runs algorithm passing in dates and bool.  General way algorithm is called.
		/// </summary>
		/// <param name="assignAll">Decide if assign using dates or just assign all offers within the year</param>
		/// <param name="startDate">Start date of algorithm</param>
		/// <param name="endDate">End date of algorithm</param>
		/// <returns></returns>
        [HttpPost]
        public ActionResult showResults(Boolean assignAll, System.DateTime startDate, System.DateTime endDate) {
			dateStart = startDate;
			dateEnd = endDate;

            AssignReferees();
            modelResult.sortResult();
            return View(modelResult);
        }

		/// <summary>
		/// Shows admin index page
		/// </summary>
		/// <returns></returns>
		public ActionResult Index() {

			return View();
		}

		/// <summary>
		/// General class to have a referee and offer combined
		/// </summary>
		class pair {
			public int referee; //referee
			public int offer; //offer

			public pair(int off, int refs) {
				referee = refs;
				offer = off;
			}
		}

		/// <summary>
		/// Class for what fields a referee has:
		///		available: Dictionary of available offers (for this referee), int: offerID, Item: contains further information if needed on offer in relation to ref
		///		unavailable: Dictionary of unavailable offers (for this referee), int: offerID, Item: contains further information if needed on offer in relation to ref
		///		assignedTo: Hashset of ints which are offerIds which this ref is assigned to
		///		canAssign: How many offers the referee can be assigned to
		///		actualRef: Referee data for this referee as assigned to in db		
		/// </summary>
		class refInfo {
			public Dictionary<int, Item> available, unavailable; //available & unavailable referees/offers of that offer/referee
			public HashSet<int> assignedTo; //what that offer/referee is assigned to (offer is always only 1)
			public int canAssign; //How many offers the referee can be assigned to (offers will always be 1)
			public REFEREE actualRef;

			public refInfo(int offersAvailableToRef, REFEREE i) {
				available = new Dictionary<int, Item>();
				unavailable = new Dictionary<int, Item>();
				assignedTo = new HashSet<int>();
				actualRef = i;
				canAssign = offersAvailableToRef;
			}
		}


		/// <summary>
		///		available: Dictionary of available referees (for this offer), int: refID, Item: contains further information if needed on referee in relation to this offer
		///		unavailable: Dictionary of unavailable referees (for this offer), int: refID, Item: contains further information if needed on referee in relation to this offer
		///		assignedTo: refId of offer it is assigned to.  -1 if unassigned
		///		actualOffer: Referee data for this offer as assigned to in db
		/// </summary>
		class offerInfo {
			public Dictionary<int, Item> available, unavailable; //available & unavailable referees/offers of that offer/referee
			public int assignedTo;
			public OFFER actualOffer;

			public offerInfo(OFFER i) {
				available = new Dictionary<int, Item>();
				unavailable = new Dictionary<int, Item>();
				assignedTo = -1;
				actualOffer = i;
			}
		}

		/// <summary>
		/// Further information needed for either offer/referee for a single referee/offer
		///		tabu: How many turns should the referee/offer pair not be assigned to again
		/// </summary>
		class Item {
			public int tabu; //At what temperature this item will not be tabu anymore

			public Item() {
				tabu = -1;
			}
		}

		Dictionary<int, offerInfo> dOffers;  //Dictionary of offers that need to be assigned
		Dictionary<int, refInfo> dReferees; //Dictionary of referees that can be assigned
		LinkedList<pair> llCompleted; //List of assigned offers/referee pairs
		HashSet<int> hFilledOffers; //hashset of filled offers (so can be easily unassigned)
		HashSet<int> hCanBeFilledOffers; //Hashset of offers which have available referees (so can easily fill to be position)
		Dictionary<int, HashSet<int>> dQualificationRefStorage; //Dictionary of an int (qualificationID), and a Hashset of referee Id's with that qualification ID
		Dictionary<int, OFFER> dOfferStorage; //storage of actual offers that need to be assigned
		Dictionary<int, REFEREE> dRefereeStorage; //storage of actual referees that need to be assigned
		Dictionary<int, MATCH> dMatchStorage; //storage of actual matches that have offers
		List<Dictionary<int, offerInfo>> bestOffers; //storage of the dOffers at the best point in time -> Used for if the current set fails the sim annealing part to reset it
		List<Dictionary<int, refInfo>> bestReferees; //storage of dReferees at the best point in time -> Used if the current set fails the sim annealing to reset it
		Dictionary<int, Dictionary<int, bool>> matchClashes; //Dictionary of int (match id), with a dictionary of int (other matches), and whether they clash or not
		Dictionary<int, int> dGamesAvailableToRef; //dictionary of int (referee IDs) to how many games they can ref for the day
		DateTime dateCurrent, dateStart, dateEnd; //Dates of start/current/end of the algorithm


		Raoconnection db; //db connection
		AlgorithmModel modelResult; //model that is required to fill to show to user once algorithm has completed
		int maxOffersFilled; //Max amount of offers that can be filled
		int currOffersFilled; //current number of offers filled
		long initTemp; //How many iterations of search to go through
		long currTemp; //What current iteration (temperature it is)
		int bestOffersFilled; //number of offers filled at best point in time
		Random rand; //random number generator
		int currPriorityFilled; //current priority filled
		int bestPriorityFilled; //best priority filled
		int maxPriorityFilled; //best case scenario of priority filled

		/// <summary>
		/// Initialise the global variable for the entire algorithm
		/// </summary>
		void initGlobalVars() {
			db = new Raoconnection();
			rand = new Random();
			llCompleted = new LinkedList<pair>();
			dQualificationRefStorage = new Dictionary<int, HashSet<int>>();
			dRefereeStorage = new Dictionary<int, REFEREE>();
		}

		/// <summary>
		/// Initialise or reset the global variables for a day by day basis
		/// </summary>
		void resetGlobalVars() {
			dOffers = new Dictionary<int, offerInfo>();
			dReferees = new Dictionary<int, refInfo>();
			hFilledOffers = new HashSet<int>();
			hCanBeFilledOffers = new HashSet<int>();
			dOfferStorage = new Dictionary<int, OFFER>();
			bestOffersFilled = 0;
			maxOffersFilled = 0;
			currOffersFilled = 0;
			initTemp = 1000;
			currPriorityFilled = 0;
			maxPriorityFilled = 0;
			bestPriorityFilled = 0;
			dGamesAvailableToRef = new Dictionary<int, int>();
			dMatchStorage = new Dictionary<int, MATCH>();
			matchClashes = new Dictionary<int, Dictionary<int, bool>>();
		}

		/// <summary>
		/// Checks if the offer/referee pair is tabu or not
		/// </summary>
		/// <param name="oID">Offer ID</param>
		/// <param name="rID">Referee ID</param>
		/// <returns></returns>
		bool checkTabu(int oID, int rID) { //check if this is tabu
			if (dOffers[oID].available[rID].tabu > currTemp) {
				dOffers[oID].available[rID].tabu -= 1; //minus one from tabu
				dReferees[rID].available[oID].tabu -= 1;
				return true;
			}
			else
				return false;
		}

		/// <summary>
		/// Checks if a referee can fill an offer at the current point in time by:
		///		Checking if referee has been assigned to max amount of offers he can be
		///		Checking if offer is currently filled
		///		Checking if referee is assigned to an offer which clashes with the offer it is checking
		/// </summary>
		/// <param name="oID">Offer ID</param>
		/// <param name="rID">Referee ID</param>
		/// <returns>
		///		true: If referee can be assigned to offer
		///		false: If referee cannot be assigned to offer
		/// </returns>
		bool checkAvailable(int oID, int rID) { //check if this ref is available to referee this offer (mostly for conflicts timewise)
			if (dReferees[rID].canAssign == dReferees[rID].assignedTo.Count()) { //referee is assigned to max
				return false;
			}
			if (dOffers[oID].assignedTo != -1) {//Offer is already filled
				return false;
			}
			foreach (var i in dReferees[rID].assignedTo) {
				if (dOfferStorage[i].matchId == dOfferStorage[oID].matchId) //if offers are on the same match
					return false;
				if (!matchClashes.ContainsKey(dOfferStorage[i].matchId)) { //if matchClashes does not hold both match ID's
					calculateClash(dOfferStorage[oID].matchId, dOfferStorage[i].matchId); //calculate if it clashes
				}
				else if (!matchClashes[dOfferStorage[i].matchId].ContainsKey(dOfferStorage[oID].matchId)) { //if matchClashes holds only one matchID and not the other
					calculateClash(dOfferStorage[oID].matchId, dOfferStorage[i].matchId); //calculate if it clashes
				}
				if (matchClashes[dOfferStorage[i].matchId][dOfferStorage[oID].matchId] == true) { //if it holds both id's
					return false;
				}
			}
			return true;
		}

		/// <summary>
		/// Mark the offerID/refereeID pair as unavailable (as it has already checked and said it is not)
		/// </summary>
		/// <param name="oID">offerID</param>
		/// <param name="rID">refereeID</param>
		void setPairUnavailable(int oID, int rID) {
			bool canBeFilled = (dOffers[oID].available.Count() > 0); //saves whether the offer has available referees
			if (!dOffers[oID].available.ContainsKey(rID)) { //checks if the offer/referee pair is already marked as unavailable
				return; //ALREADY REMOVED -> Most common cause is when it is the offer you assigned
			}
			
			//add the offer/referee to unavailable part and remove from available
			dOffers[oID].unavailable.Add(rID, dOffers[oID].available[rID]);
			dOffers[oID].available.Remove(rID); 
			dReferees[rID].unavailable.Add(oID, dReferees[rID].available[oID]);
			dReferees[rID].available.Remove(oID);


			if (canBeFilled) { //if it could be filled before
				if (dOffers[oID].available.Count() == 0) { //and can't be now
					hCanBeFilledOffers.Remove(oID); //remove from CanBeFilled
				}
			}
		}

		/// <summary>
		/// Mark referee/offer pair as available (already checked if it can be)
		/// </summary>
		/// <param name="oID">offerID</param>
		/// <param name="rID">refereeID</param>
		void setPairAvailable(int oID, int rID) {
			bool canBeFilled = (dOffers[oID].available.Count() > 0); //save state whether it can be filled or not

			//mark referee/offer pair as available then remove from unavailable
			dOffers[oID].available.Add(rID, dOffers[oID].unavailable[rID]);
			dOffers[oID].unavailable.Remove(rID);
			dReferees[rID].available.Add(oID, dReferees[rID].unavailable[oID]);
			dReferees[rID].unavailable.Remove(oID);
			
			
			if (!canBeFilled) { //if couldn't be filled before
				if (dOffers[oID].available.Count() > 0) { //and can be now
					hCanBeFilledOffers.Add(oID); //add to canbefilled
				}
			}
		}

		/// <summary>
		/// Used when adding or removing a referee to update the status of the other offers/referees in relation to that pair
		/// </summary>
		/// <param name="addRemove">
		///		true: if adding a referee to an offer
		///		false: if removing a referee from an offer
		/// </param>
		/// <param name="oID">offerID</param>
		/// <param name="rID">refereeID</param>
		void updateAvailability(bool addRemove, int oID, int rID) { //Update availability of referee
			if (addRemove) { //if assigning a referee to an offer
				setOfferUnavailable(oID); //set offer as unavailable to all referees
				
				if (dReferees[rID].canAssign == dReferees[rID].assignedTo.Count()) { //if referee is assigned to max amount of referees
					setRefereeUnavailable(rID); //mark referee as completely unavailable
					return; //(set offer and referee unavailable)
				}

				LinkedList<int> nuke = new LinkedList<int>(); //save the offers that need to be marked unavailable
				foreach (var i in dReferees[rID].available) { //for each available offer for that referee
					if (!checkAvailable(i.Key, rID)) { //check if offer is still available for the referee
						nuke.AddLast(i.Key); //if not mark to set pair as unavailable
					}
				}
				foreach (var i in nuke) {
					setPairUnavailable(i, rID); //set pair unavailable
				}
			}

			else { //if unassigning a referee from an offer
				LinkedList<int> nuke = new LinkedList<int>(); //used to save offer/ref id's marked for an action
				foreach (var i in dOffers[oID].unavailable) { //for each referee marked as unavailable for that offer (offer was just filled so all of them)
					if (checkAvailable(oID, i.Key)) { //check if referee can now fill that offer
						nuke.AddLast(i.Key); //mark to set available
					}
				}
				foreach (var i in nuke) {
					setPairAvailable(oID, i); //set as available
				}
				nuke = new LinkedList<int>(); //reset the list
				foreach (var i in dReferees[rID].unavailable) { //for each offer that was marked as unavailable
					if (checkAvailable(i.Key, rID)) { //check if ref can now be assigned to that offer
						nuke.AddLast(i.Key);
					}
				}
				foreach (var i in nuke) {
					setPairAvailable(i, rID);
				}
			}
		}

		/// <summary>
		/// Mark referee as unavailable to all offers
		/// </summary>
		/// <param name="rID">refereeID</param>
		void setRefereeUnavailable(int rID) { //Referee has been assigned to canAssign offers
			LinkedList<int> nuke = new LinkedList<int>();
			foreach (var i in dReferees[rID].available) { //for each offer marked as available
				nuke.AddLast(i.Key); //mark to set unavailable
			}
			foreach (var i in nuke) {
				setPairUnavailable(i, rID); //set as unavailable
			}
		}

		/// <summary>
		/// Set offer as unavailable to be filled to all referees: Most likely offer has been filled
		/// </summary>
		/// <param name="oID">offerID</param>
		void setOfferUnavailable(int oID) {
			LinkedList<int> nuke = new LinkedList<int>();
			foreach (var i in dOffers[oID].available) { //for each referee that has offer marked as available
				nuke.AddLast(i.Key); //mark to set it as unavailable
			}
			foreach (var i in nuke) {
				setPairUnavailable(oID, i); //set as unavailable
			}
		}

		/// <summary>
		/// Calls from db the offers that are needed to be filled
		/// Also saves the qualifications needed that are needed to fill those offers
		/// </summary>
		void getOffersAndQualificationIDs() {
			foreach (var i in db.OFFERs) {
				if (i.status == 5 && i.MATCH.matchDate.Date == dateCurrent.Date) { //get offers where status is SmartAssign and date is the current date
					dOfferStorage.Add(i.offerId, i); //store offer by offerID
					foreach (var k in i.OFFERQUALs) { //for each qualification in offer
						if (!dQualificationRefStorage.ContainsKey(k.qualificationId)) //if not already stored the qualification ID
							dQualificationRefStorage.Add(k.qualificationId, new HashSet<int>()); //Store the qualification ID
					}
					dOffers.Add(i.offerId, new offerInfo(i)); //store offer ID, and offerInfo and pass the actual offer
					maxPriorityFilled += i.priority; //add priority to count best possible priority
				}
			}
		}

		/// <summary>
		/// Returns how many games on current date the referee is assigned to (pending or accepted)
		/// </summary>
		/// <param name="rID">refereeID</param>
		/// <returns>int of offers already assigned to on that specific date</returns>
		int countOffersAlreadyAssigned(int rID) {
			return db.OFFERs.Where(o => o.refId == rID && (o.status == 1 || o.status == 3) && o.dateOfOffer == dateCurrent).Count();
		}


		/// <summary>
		/// Gets all referees and stores them that have a qualification needed by an offer and adds them to a list which can be easily accessed to get all referees with a specific qualification
		/// </summary>
		void fillRefereeByQualification() {
			foreach (var i in dQualificationRefStorage) { //For each qualification that an offer needs
				foreach (var j in db.REFEREEs) { //for each referee
					foreach (var k in j.USERQUALs) { //for each qualification a referee has
						if (i.Key == k.qualificationId) { //if qualification is needed
							if (!dRefereeStorage.ContainsKey(j.refId)) { //save referee if not already stored
								dRefereeStorage.Add(j.refId, j);
							}
							if (!dGamesAvailableToRef.ContainsKey(j.refId)) { //count games already assigned to him if not already stored
								dGamesAvailableToRef.Add(j.refId, j.maxGames - countOffersAlreadyAssigned(j.refId));
							}
							if (dGamesAvailableToRef[j.refId] > 0) //add as having qualification if available to referee at least one game
								i.Value.Add(j.refId);

						}
					}
				}
			}
		}

		/// <summary>
		/// changes offer ID to the date of the offer
		/// </summary>
		/// <param name="oID">offerID</param>
		/// <param name="rID">refereeID</param>
		/// <returns></returns>
		bool containsOneOff(int oID, int rID) {
			return containsOneOff(db.OFFERs.Find(oID).dateOfOffer, rID);
		}

		/// <summary>
		/// Checks if a referee contains a one-off availability on a date 
		/// </summary>
		/// <param name="matchDateTime">Date of the match</param>
		/// <param name="rID">refereeID</param>
		/// <returns></returns>
		bool containsOneOff(DateTime matchDateTime, int rID) { 
			try {
				var temp = db.OneOffAVAILABILITies.Find(rID, matchDateTime.Date); 
				if (temp == null) //if it can't find it
					return false;
				else
					return true; //if it found one
			}
			catch {
				return false; //if it can't find it/had an exception
			}
		}

		/// <summary>
		/// converts offerID to date of match of offer
		/// </summary>
		/// <param name="oID">offerID</param>
		/// <param name="rID">refereeID</param>
		/// <returns></returns>
		bool checkOneOff(int oID, int rID) {
			return checkOneOff(db.OFFERs.Find(oID).dateOfOffer, rID);
		}

		/// <summary>
		/// Returns if one-off referee has is available or not available
		/// </summary>
		/// <param name="matchDateTime">dateTime of Match</param>
		/// <param name="rID">refereeID</param>
		/// <returns>
		///		true: Available
		///		false: Unavailable
		/// </returns>
		bool checkOneOff(DateTime matchDateTime, int rID) {
			try {
				var temp = db.OneOffAVAILABILITies.Find(rID, matchDateTime.Date);
				if (temp.timeOnOrOff == true)
					return true;
				else
					return false;
			}
			catch (SystemException a) {
				return false;
			}

		}

		/// <summary>
		/// Gets the availability for day of datetime for a referee
		/// </summary>
		/// <param name="dt">dateTime</param>
		/// <param name="rID">refereeID</param>
		/// <returns>
		///		The weekly availability of the date time for a referee -> Refer to data dictionary for actual values
		/// </returns>
		int getWeeklyAvailabilityForDay(DateTime dt, int rID) {
			try {
				switch (dt.DayOfWeek) {
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
			catch {
				return 0;
			}
		}


		/// <summary>
		/// Checks the weekly availability against the dateTime of match
		/// </summary>
		/// <param name="weeklyAvailability">Weekly Availability of referee (refer to data dictionary)</param>
		/// <param name="matchDateTime">DateTime of Referee</param>
		/// <returns>
		///		true: Available
		///		false: Unavailable
		/// </returns>
		bool checkWeeklyAvailabilityForMatch(int weeklyAvailability, DateTime matchDateTime) {
			if (weeklyAvailability == 0) {
				return false;
			}
			if (weeklyAvailability >= 8) {
				if (matchDateTime.TimeOfDay < new TimeSpan(6, 0, 0)) {
					return true;
				}
				weeklyAvailability -= 8;
			}
			else if (matchDateTime.TimeOfDay < new TimeSpan(6, 0, 0)) {
				return false;
			}
			if (weeklyAvailability >= 4) {
				if (matchDateTime.TimeOfDay < new TimeSpan(12, 0, 0)) {
					return true;
				}
				weeklyAvailability -= 4;
			}
			else if (matchDateTime.TimeOfDay < new TimeSpan(12, 0, 0)) {
				return false;
			}
			if (weeklyAvailability >= 2) {
				if (matchDateTime.TimeOfDay < new TimeSpan(18, 0, 0)) {
					return true;
				}
				weeklyAvailability -= 2;
			}
			else if (matchDateTime.TimeOfDay < new TimeSpan(18, 0, 0)) {
				return false;
			}
			if (weeklyAvailability >= 1) {
				if (matchDateTime.TimeOfDay < new TimeSpan(24, 0, 0)) {
					return true;
				}
				weeklyAvailability -= 1;
			}
			else if (matchDateTime.TimeOfDay < new TimeSpan(24, 0, 0)) {
				return false;
			}
			throw new SystemException();
		}


		/// <summary>
		///		Check if referee is available to do the match (without assigning any new offers to him/her)
		/// </summary>
		/// <param name="oID">offerID</param>
		/// <param name="rID">refereeID</param>
		/// <returns>
		///		true: Available
		///		false: Unavailable
		/// </returns>
		bool checkInitAvailability(int oID, int rID) {
			DateTime matchDateTime = dOfferStorage[oID].MATCH.matchDate; //get date

			if (containsOneOff(matchDateTime, rID)) { //check if has one off
				if (checkOneOff(matchDateTime, rID))
					return true;
				else
					return false;
			}
			else { //Check Daily Availability
				int weeklyAvailabilityForDay = getWeeklyAvailabilityForDay(matchDateTime, rID);
				return checkWeeklyAvailabilityForMatch(weeklyAvailabilityForDay, matchDateTime);
			}
		}

		/// <summary>
		/// Add referee with rID to offer with oID as referee is available
		/// </summary>
		/// <param name="oID">offerID</param>
		/// <param name="rID">refereeID</param>
		void addInitialRefereesToOffer(int oID, int rID) {
			dOffers[oID].available.Add(rID, new Item()); //add referee to offer
			if (!dReferees.ContainsKey(rID)) { //check if is first time referee has been assigned to an offer
				if (!dGamesAvailableToRef.ContainsKey(rID)) { //count games already assigned to him if not already stored
					dGamesAvailableToRef.Add(rID, dRefereeStorage[rID].maxGames - countOffersAlreadyAssigned(rID));
				}
				dReferees.Add(rID, new refInfo(dGamesAvailableToRef[rID], dRefereeStorage[rID])); //add referee to storage, with gamesAvailableToRef, and the actual Referee
				maxOffersFilled += dReferees[rID].canAssign; //count what top number of offers to fill is
			}
			dReferees[rID].available.Add(oID, new Item()); //add offer to referee
		}

		/// <summary>
		/// Add referees who have qualifications and are available for the offer
		/// Checks:
		///		Qualifications & Levels
		///		Availability
		/// </summary>
		void addRefereesToOffers() {
			foreach (var i in dOfferStorage) { //for each offer that needs to be assigned
				HashSet<int> availableReferees = null; 
				if (i.Value.OFFERQUALs.Count() > 0) { //if has qualification
					foreach (var j in i.Value.OFFERQUALs) { //for each qualication in offer
						if (availableReferees == null) { //if it's the first qualification
							availableReferees = new HashSet<int>();
							foreach (var k in dQualificationRefStorage[j.qualificationId]) { //each referee with that qualification
								if (dRefereeStorage[k].USERQUALs.Where(qual => qual.qualificationId == j.qualificationId).First().qualLevel >= j.qualLevel) { //if referee qualification level is >= required qualification level -> Should store in qualStorage
									if (checkInitAvailability(i.Key, k)) { //check if referee is available
										availableReferees.Add(k); //add that referee
									}
								}
							}
						}
						else { //if not first qualification
							HashSet<int> temp = availableReferees; //save current refs
							availableReferees = new HashSet<int>(); //set available refs to null
							foreach (var k in temp) { //each ref previously available
								if (dQualificationRefStorage[j.qualificationId].Contains(k)) { //if ref prev available has next qualification
									if (dRefereeStorage[k].USERQUALs.Where(qual => qual.qualificationId == j.qualificationId).First().qualLevel >= j.qualLevel) { //if referee qualification level is >= required qualification level -> Should store in qualStorage
										availableReferees.Add(k); //add ref as available
									}
								}
							}
						}
						if (availableReferees.Count() == 0) { //if no referees are available
							break; //stop checking
						}
					}
				}
				else { //if no qualification
					availableReferees = new HashSet<int>();
					foreach (var j in db.REFEREEs) { //check every referee
						if (!dRefereeStorage.ContainsKey(j.refId)) {
							dRefereeStorage.Add(j.refId, j);
							dGamesAvailableToRef.Add(j.refId, j.maxGames - countOffersAlreadyAssigned(j.refId));
						}
						if (checkInitAvailability(i.Key, j.refId)) {
							availableReferees.Add(j.refId);
						}
					}
				}

				if (availableReferees.Count == 0) { //if there are no available referees for an offer
					llCompleted.AddLast(new pair(i.Key, -1)); //add offer as completed unassigned
					dOffers.Remove(i.Key); //remove from processing
				}
				else {
					foreach (var j in availableReferees) { //for every referee that is available
						addInitialRefereesToOffer(i.Key, j); //add them
					}
				}
			}
		}

		/// <summary>
		/// Stub for calculating how long it will take for a referee to travel in between the offers.
		/// Need to implement google API to check time to travel between each location
		/// </summary>
		/// <param name="i">matchID of initial match</param>
		/// <param name="j">matchID of match to travel to</param>
		/// <returns>
		///		Time required between games in minutes
		///			0 if at same location
		///			30 if not at same location
		/// </returns>
		int calcTimeBufferBetweenGames(int i, int j) {
			if (dMatchStorage[i].locationId == dMatchStorage[j].locationId) {
				return 0;
			}
			else {
				return 30; // CALCULATE TIME 
			}
		}

		/// <summary>
		/// Calculate if 2 matches clash (same time period)
		/// </summary>
		/// <param name="i">matchID of first match</param>
		/// <param name="j">matchID of second match</param>
		/// <returns>
		///		true: If matches clash
		///		false: If matches don't clash
		/// </returns>
		bool calcMatchTimeClash(int i, int j) {
			if (!dMatchStorage.ContainsKey(i)) { //if matchStorage doesn't have the matchID
				dMatchStorage.Add(i, db.MATCHes.Find(i)); //Save the matchID in storage
			}
			if (!dMatchStorage.ContainsKey(j)) {//if matchStorage doesn't have the matchID
				dMatchStorage.Add(j, db.MATCHes.Find(j)); //Save the matchID in storage
			} 
			int buffer = calcTimeBufferBetweenGames(i, j); //calculate time in minutes required to travel between matches
			if (dMatchStorage[i].matchDate.Date == dMatchStorage[j].matchDate.Date) { //if matches are on the same date (will always be true on this iteration of algorithm as algorithm does day by day)
				if (dMatchStorage[i].matchDate < dMatchStorage[j].matchDate) { //if first match is before second match
					if (dMatchStorage[i].matchDate.AddMinutes(dMatchStorage[i].matchLength + buffer) <= dMatchStorage[j].matchDate) //if first match + matchlength + buffer is after the second match start
						return false;
					else
						return true;
				}
				else if (dMatchStorage[i].matchDate > dMatchStorage[j].matchDate) { //if second match is before the first match
					if (dMatchStorage[i].matchDate >= dMatchStorage[j].matchDate.AddMinutes(dMatchStorage[j].matchLength + buffer)) // if second match + length + buffer is after the first match start
						return false;
					else
						return true;
				}
				else { // if both matches at the same time
					return true;
				}
			}
			else {
				return false;
			}
		}


		/// <summary>
		/// Calculate if 2 matches clash
		/// </summary>
		/// <param name="i">matchID of first match</param>
		/// <param name="j">matchID of second match</param>
		void calculateClash(int i, int j) {
			if (matchClashes.ContainsKey(i)) {
				if (matchClashes[i].ContainsKey(j)) { //have already calculated
					return;
				}
				else { //haven't calculated
					bool tmp = calcMatchTimeClash(i, j);
					matchClashes[i].Add(j, tmp);
					if (!matchClashes.ContainsKey(j)) {
						matchClashes.Add(j, new Dictionary<int, bool>());
					}
					matchClashes[j].Add(i, tmp);
				}
			}
			else { //haven't calculated
				bool tmp = calcMatchTimeClash(i, j);
				matchClashes.Add(i, new Dictionary<int, bool>());
				matchClashes[i].Add(j, tmp);
				if (!matchClashes.ContainsKey(j)) {
					matchClashes.Add(j, new Dictionary<int, bool>());
				}
				matchClashes[j].Add(i, tmp);
			}
		}


		/// <summary>
		/// Calculate clashes for all matches between each other.  Should remove function and calculate them if required in runtime
		/// </summary>
		void calculateClashes() {
			foreach (var i in dReferees) { //for each referee
				foreach (var j in i.Value.available) { //for each offer in referee
					foreach (var k in i.Value.available) { //for each offer in referee (checking each offer against every other offer -> NOT EFFICIENT
						if (dOfferStorage[j.Key].MATCH.matchId != dOfferStorage[k.Key].MATCH.matchId) { //if it isn't the same match
							calculateClash(dOfferStorage[j.Key].MATCH.matchId, dOfferStorage[k.Key].MATCH.matchId); //calculate if the 2 matches clash
						}
					}
				}
			}
		}

		/// <summary>
		/// Fill the initial sets:
		///		Gets offers and Qualifications
		///		Fills Referees by Qualifications
		///		Adds the Referees to dOffers
		///		Calculates clashes between matches can make more efficient by removing this
		/// </summary>
		void fillSets() {
			getOffersAndQualificationIDs();
			fillRefereeByQualification();
			addRefereesToOffers();
			calculateClashes();
		}

		/// <summary>
		/// Remove an offer from algorithm -> Generally used for if there is no referee that can complete or referee can complete without it affecting anything else
		/// </summary>
		/// <param name="oID">offerID of offer to remove</param>
		void removeOffer(int oID) { 
			if (dOffers.ContainsKey(oID)) { //Check if it hasn't been removed already
				foreach (var i in dOffers[oID].available) { //for each referee that can do this
					dReferees[i.Key].available.Remove(oID);//remove the offer from his availability
					if (dReferees[i.Key].available.Count() == 0) { //if only offer that referee could have completed
						maxOffersFilled -= dReferees[i.Key].canAssign; //remove the referee
						dReferees.Remove(i.Key);
					}
				}
				dOffers.Remove(oID);
			}
		}


		/// <summary>
		/// Randomise the order of the dictionary - Unknown source
		/// Call by using a foreach (var i in UniqueRandomValues(ICollectible)) -> Will output the values in a random order
		/// </summary>
		/// <param name="dict">Dictionary that you want values from</param>
		/// <returns>values from the dictionary in a random order</returns>
		public IEnumerable<TKey> UniqueRandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict) {
			// Put the values in random order
			Random rand = new Random();
			LinkedList<TKey> values = new LinkedList<TKey>(from v in dict.Keys
														   orderby rand.Next()
														   select v);
			// Remove the values one at a time
			while (values.Count > 0) {
				yield return values.Last.Value;
				values.RemoveLast();
			}
		}

		/// <summary>
		/// Assign a referee to an offer (has been checked if available)
		/// </summary>
		/// <param name="oID">offerID of offer to assign referee too</param>
		/// <param name="rID">refereeID of referee to assign</param>
		public void assign(int oID, int rID) {
			dOffers[oID].assignedTo = rID;
			dReferees[rID].assignedTo.Add(oID);
			updateAvailability(true, oID, rID); //update availability
			currOffersFilled++;
			currPriorityFilled += dOffers[oID].actualOffer.priority;
			hFilledOffers.Add(oID);
		}

		/// <summary>
		/// Returns a value for how long a offer/referee pair should be tabu
		/// </summary>
		/// <param name="oID">ID of item you want to make tabu</param>
		/// <returns>The temperature at when it will no longer be tabu</returns>
		long calcTabu(int oID) {
			return (initTemp / ((initTemp - currTemp) / ((long)dOffers[oID].available.Count() + (long)dOffers[oID].unavailable.Count())));
		}

		/// <summary>
		/// Mark an offer/referee pair as tabu (usually when unassigning)
		/// </summary>
		/// <param name="oID">offerID of offer you want to make tabu</param>
		/// <param name="rID">refereeID of referee you want to make tabu</param>
		void setTabu(int oID, int rID) {
			int amount = (int)calcTabu(oID);
			dOffers[oID].available[rID].tabu += amount;
			dReferees[rID].available[oID].tabu += amount;
		}

		/// <summary>
		/// Make an offer unassigned
		/// </summary>
		/// <param name="oID">offerID you want to unassign</param>
		void unassign(int oID) {
			int rID = dOffers[oID].assignedTo;
			hFilledOffers.Remove(oID);
			dOffers[oID].assignedTo = -1;
			dReferees[rID].assignedTo.Remove(oID);
			updateAvailability(false, oID, rID); //updateAvailability
			setTabu(oID, rID); //set tabu
			currOffersFilled--;
			currPriorityFilled -= dOffers[oID].actualOffer.priority;
		}


		/// <summary>
		/// Get a random available referee for an offer
		/// </summary>
		/// <param name="oID"></param>
		/// <returns></returns>
		int randomAvailableRef(int oID) {
			Random rand = new Random();
			foreach (var i in UniqueRandomValues(dOffers[oID].available)) { //randomly go through the available referees
				if (!checkTabu(oID, i)) { //check if they have tabu
					return i; //if not tabu then return ref
				}
			}
			return dOffers[oID].available.ElementAt(rand.Next(0, dOffers[oID].available.Count)).Key; //if all have tabu, assign random
		}

		/// <summary>
		/// Preprocess for algorithm:
		///		if referee can ref all games assigned to him -> Make him do so (won't affect other referees)
		///		Randomly assign all referees best possible for starting position
		///		Set MaxOffersFilled (to count if all offers have been filled)
		/// </summary>
		private void initChecks() {
			LinkedList<int> nuke = new LinkedList<int>();
			do {
				foreach (var i in nuke) { //remove offers that have been finished
					removeOffer(i);
				}
				nuke.Clear();
				foreach (var i in dReferees) {
					if (i.Value.available.Count <= i.Value.canAssign) { //if the ref is available to referee all games the can be assigned
						foreach (var k in i.Value.available) {
							bool notDouble = true;
							foreach (var j in llCompleted) {
								if (j.offer == k.Key) {
									notDouble = false;
								}

							}
							if (notDouble) {
								llCompleted.AddLast(new pair(k.Key, i.Key)); //mark as completed
								nuke.AddLast(k.Key); //mark to remove the offer
								break;
							}
						}
					}
				}
			} while (nuke.Count() != 0); //can do multiple times as offers will be removed 

			foreach (var i in dOffers) {
				hCanBeFilledOffers.Add(i.Key);
			}

			Random rand = new Random();
			foreach (var i in UniqueRandomValues(dOffers)) { //randomise offers 
				if (dOffers[i].available.Count() > 0) {
					assign(i, dOffers[i].available.ElementAt(rand.Next(0, dOffers[i].available.Count)).Key); //Assign to an offer i, a randomly selected ref from available referees
				}
			}

			if (maxOffersFilled > dOffers.Count()) //if refs can fill more offers than there are
				maxOffersFilled = dOffers.Count();

			if (currOffersFilled > bestOffersFilled) {
				bestOffersFilled = currOffersFilled;
				bestOffers = new List<Dictionary<int, offerInfo>>();
				bestReferees = new List<Dictionary<int, refInfo>>();
				saveState();
			}
		}

		/// <summary>
		/// Calculate how many offers to reset at a current point in time based off how many offers there are and how long algorithm has been running
		/// </summary>
		/// <returns>Int of how many offers to reset</returns>
		long calcOffersToReset() {
			if (dOffers.Count() < 20)
				return 1;
			return (long)(((0.2 * dOffers.Count()) * (initTemp / currTemp)) + 1);
		}

		/// <summary>
		/// Unassign a random offer (which has been filled)
		/// </summary>
		void unassignRandom() {
			Random rand = new Random();
			unassign(hFilledOffers.ElementAt(rand.Next(0, hFilledOffers.Count())));
		}

		/// <summary>
		/// Save state -> Currently stores 3 states -> Used to reset back to previous best state or to say what the best set of refs it found
		/// </summary>
		void saveState() {
			if (bestOffers.Count > 2) { // if there are 3 elements remove one before inserting
				bestOffers.RemoveAt(rand.Next(0, 3));
			}
			bestOffers.Add(new Dictionary<int, offerInfo>(dOffers));
			bestReferees.Add(new Dictionary<int, refInfo>(dReferees));
		}

		/// <summary>
		/// reset the state back to the best previous state
		/// </summary>
		void resetState() {
			Random rand = new Random();
			int chooseState = rand.Next(0, bestReferees.Count());
			dOffers = new Dictionary<int, offerInfo>(bestOffers[chooseState]);
			dReferees = new Dictionary<int, refInfo>(bestReferees[chooseState]);
		}

		/// <summary>
		///		Decided whether to accept or deny the current solution (as in continue or reset back to best solution) - Based off Simulated Annealing
		/// </summary>
		/// <param name="dif">Difference between current priority filled and best priority filled</param>
		/// <returns>
		///		true: Accepted difference
		///		false: Denied difference (reset state)
		/// </returns>
		bool SimulatedAnnealing(int dif) {
			double difPercent = dif / maxPriorityFilled;
			double currTempPercent = currTemp / initTemp;
			if (rand.Next(0, 101) > ((currTempPercent + difPercent) * 100))
				return true; //accept
			else
				return false; //deny

		}

		/// <summary>
		///		Core part of the algorithm -> Performs the actual algorithm and calculates the best case it can find
		///		Rough Pseudocode:
		///			while (below a certain number of runs & not found a case where all offers filled {
		///				unassign a number of offers
		///				fill all possible offers randomly
		///				If  better case than best found case
		///					save and continue
		///				else if equal
		///					continue (possibly save)
		///				else if worse
		///					possibly reset
		///			}
		///					
		/// </summary>
		void performAlgorithm() {
			for (currTemp = 0; currTemp < initTemp && bestOffersFilled < maxOffersFilled; currTemp++) {
				long countAssign = calcOffersToReset(); //Calc how many to unassign
				for (long i = 0; i < countAssign; i++) { 
					unassignRandom();
				}
				while (hCanBeFilledOffers.Count() > 0) { //while offers can be filled
					int currOffer = hCanBeFilledOffers.ElementAt(rand.Next(0, hCanBeFilledOffers.Count())); //assign to a random referee who is available
					assign(currOffer, randomAvailableRef(currOffer));
				}

				if (currOffersFilled > bestOffersFilled) {
					bestOffersFilled = currOffersFilled;
				}

				if (currPriorityFilled > bestPriorityFilled) {
					bestPriorityFilled = currPriorityFilled;
					bestOffers = new List<Dictionary<int, offerInfo>>();
					bestReferees = new List<Dictionary<int, refInfo>>();
					saveState();
				}
				else if (currPriorityFilled == bestPriorityFilled) {
					saveState();
				}
				else { //worse than best
					if (!SimulatedAnnealing(bestPriorityFilled - currPriorityFilled))
						resetState();
				}
			}
		}

		/// <summary>
		/// Put the results of the algorithm in a format that can be displayed on a webpage
		/// </summary>
		void setModel() {
			modelResult = new AlgorithmModel(1);
			foreach (var i in llCompleted) {
				modelResult.result[0].pairs.Add(new Models.pair(i.offer, i.referee, db));
			}
		}

		/// <summary>
		/// Save the best results of bestOffers to llCompleted
		/// </summary>
		void saveResults() {
			if (bestOffers != null) {
				foreach (var i in bestOffers.First()) {
					llCompleted.AddLast(new pair(i.Key,i.Value.assignedTo));
				}
			}
		}

		/// <summary>
		/// The main function to call to start the algorithm -> Function is self-explanatory
		/// </summary>
		public void AssignReferees() {
			initGlobalVars();
			for (dateCurrent = dateStart; dateCurrent <= dateEnd; dateCurrent = dateCurrent.AddDays(1)) {
				resetGlobalVars();
				fillSets();
				initChecks();
				performAlgorithm();
				saveResults();
			}
			setModel();

			//db.Entry(of).State = EntityState.Modified;
			//db.SaveChanges();
		}

		/// <summary>
		/// Function to get referees available for an offer outside of algorithm -> Built for Connor
		/// </summary>
		/// <param name="oID">offerID of offer you want to find referees for</param>
		/// <returns>List of referees available to take an offer (available and has qualifications)</returns>
		public List<REFEREE> getAvailableRefereesForOffer(int oID) {
			OFFER offer = db.OFFERs.Find(oID);
			List<REFEREE> availableReferees = new List<REFEREE>();
			List<KeyValuePair<int, int>> offerQuals = new List<KeyValuePair<int, int>>();
			foreach (var i in offer.OFFERQUALs) {
				offerQuals.Add(new KeyValuePair<int, int>(i.qualificationId, i.qualLevel));
			}
			foreach (var i in db.REFEREEs) {
				bool refHasQualification = true;
				foreach (var j in offerQuals) {
					try {
						db.USERQUALs.Find(j.Key, i.refId);
					}
					catch (SystemException a) {
						refHasQualification = false;
						break;
					}
				}
				if (refHasQualification) {
					if (countOffersAlreadyAssigned(i.refId) < i.maxGames) {

						if (containsOneOff(offer.dateOfOffer, i.refId)) {
							if (checkOneOff(offer.dateOfOffer, i.refId)) {
								availableReferees.Add(i);
							}
						}
						else {
							if (checkWeeklyAvailabilityForMatch(getWeeklyAvailabilityForDay(offer.dateOfOffer, i.refId), offer.dateOfOffer)) {
								availableReferees.Add(i);
							}
						}
					}
				}
			}
			return availableReferees;
		}
	}
}
