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
			if (dOffers[oID].available[rID].tabu > 0) {
				dOffers[oID].available[rID].tabu -= 1;
				dReferees[oID].available[oID].tabu -= 1;
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
		/// 
		/// </summary>
		/// <param name="oID"></param>
		/// <param name="rID"></param>
		/// <returns></returns>
		bool containsOneOff(int oID, int rID) {
			return containsOneOff(db.OFFERs.Find(oID).dateOfOffer, rID);
		}

		bool containsOneOff(DateTime matchDateTime, int rID) { //REDO
			try {
				var temp = db.OneOffAVAILABILITies.Find(rID, matchDateTime.Date); //WRITE PRIMARY KEY FOR REFAVAILABILITY
				if (temp == null)
					return false;
				else
					return true;
			}
			catch {
				return false;
			}
		}

		bool checkOneOff(int oID, int rID) {
			return checkOneOff(db.OFFERs.Find(oID).dateOfOffer, rID);
		}

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

		bool checkInitAvailability(int oID, int rID) {
			//if (rID == 87784161)
			//return false;
			DateTime matchDateTime = dOfferStorage[oID].MATCH.matchDate;

			if (containsOneOff(matchDateTime, rID)) {
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

		void addInitialRefereesToOffer(int oID, int rID) {
			dOffers[oID].available.Add(rID, new Item());
			if (!dReferees.ContainsKey(rID)) {
				dReferees.Add(rID, new refInfo(dGamesAvailableToRef[rID], dRefereeStorage[rID])); //refereeStorage[rID].maxGames
				maxOffersFilled += dReferees[rID].canAssign; //count what top number of offers to fill is
			}
			dReferees[rID].available.Add(oID, new Item());
		}

		void addRefereesToOffers() {
			foreach (var i in dOfferStorage) {
				HashSet<int> availableReferees = null;
				if (i.Value.OFFERQUALs.Count() > 0) {
					foreach (var j in i.Value.OFFERQUALs) {
						if (availableReferees == null) {
							availableReferees = new HashSet<int>();
							foreach (var k in dQualificationRefStorage[j.qualificationId]) {
								if (checkInitAvailability(i.Key, k)) {
									availableReferees.Add(k);
								}
							}
						}
						else {
							HashSet<int> temp = availableReferees;
							availableReferees = new HashSet<int>();
							foreach (var k in temp) {
								if (dQualificationRefStorage[j.qualificationId].Contains(k)) {
									availableReferees.Add(k);
								}
							}
						}
						if (availableReferees.Count() == 0) {
							break;
						}
					}
				}
				else {
					availableReferees = new HashSet<int>();
					foreach (var j in db.REFEREEs) {
						if (!dRefereeStorage.ContainsKey(j.refId)) {
							dRefereeStorage.Add(j.refId, j);
						}
						if (checkInitAvailability(i.Key, j.refId)) {
							availableReferees.Add(j.refId);
						}
					}
				}
				if (availableReferees.Count == 0) {
					llCompleted.AddLast(new pair(i.Key, -1));
					dOffers.Remove(i.Key);
				}
				else {
					foreach (var j in availableReferees) {
						addInitialRefereesToOffer(i.Key, j);
					}
				}
			}
		}

		int calcTimeBufferBetweenGames(int i, int j) {
			if (dMatchStorage[i].locationId == dMatchStorage[j].locationId) {
				return 0;
			}
			else {
				return 30; // CALCULATE TIME 
			}
		}

		bool calcMatchTimeClash(int i, int j) {
			if (!dMatchStorage.ContainsKey(i)) {
				dMatchStorage.Add(i, db.MATCHes.Find(i));
			}
			if (!dMatchStorage.ContainsKey(j)) {
				dMatchStorage.Add(j, db.MATCHes.Find(j));
			}
			int buffer = calcTimeBufferBetweenGames(i, j);
			if (dMatchStorage[i].matchDate.Date == dMatchStorage[j].matchDate.Date) {
				if (dMatchStorage[i].matchDate < dMatchStorage[j].matchDate) {
					if (dMatchStorage[i].matchDate.AddMinutes(dMatchStorage[i].matchLength + buffer) <= dMatchStorage[j].matchDate)
						return false;
					else
						return true;
				}
				else if (dMatchStorage[i].matchDate > dMatchStorage[j].matchDate) {
					if (dMatchStorage[i].matchDate >= dMatchStorage[j].matchDate.AddMinutes(dMatchStorage[j].matchLength + buffer))
						return false;
					else
						return true;
				}
				else { // ==
					return true;
				}
			}
			else {
				return false;
			}
		}

		void calculateClash(int i, int j) {
			if (matchClashes.ContainsKey(i)) {
				if (matchClashes[i].ContainsKey(j)) { //have already calculated
					return;
				}
				else {
					bool tmp = calcMatchTimeClash(i, j);
					matchClashes[i].Add(j, tmp);
					if (!matchClashes.ContainsKey(j)) {
						matchClashes.Add(j, new Dictionary<int, bool>());
					}
					matchClashes[j].Add(i, tmp);
				}
			}
			else {
				bool tmp = calcMatchTimeClash(i, j);
				matchClashes.Add(i, new Dictionary<int, bool>());
				matchClashes[i].Add(j, tmp);
				if (!matchClashes.ContainsKey(j)) {
					matchClashes.Add(j, new Dictionary<int, bool>());
				}
				matchClashes[j].Add(i, tmp);
			}
		}

		void calculateClashes() {
			foreach (var i in dReferees) {
				foreach (var j in i.Value.available) {
					foreach (var k in i.Value.available) {
						if (dOfferStorage[j.Key].MATCH.matchId != dOfferStorage[k.Key].MATCH.matchId) {
							calculateClash(dOfferStorage[j.Key].MATCH.matchId, dOfferStorage[k.Key].MATCH.matchId);
						}
					}
				}
			}
		}

		void fillSets() {
			getOffersAndQualificationIDs();
			fillRefereeByQualification();
			addRefereesToOffers();
			calculateClashes();
		}

		void removeOffer(int oID) { //remove offer
			if (dOffers.ContainsKey(oID)) { //might've already been removed (other ref could have completed)
				foreach (var i in dOffers[oID].available) {
					dReferees[i.Key].available.Remove(oID);
					if (dReferees[i.Key].available.Count() == 0) {
						maxOffersFilled -= dReferees[i.Key].canAssign;
						dReferees.Remove(i.Key);
					}
				}
				dOffers.Remove(oID);
			}
		}

		public IEnumerable<TKey> UniqueRandomValues<TKey, TValue>(IDictionary<TKey, TValue> dict) { //randomise order of dictionary
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

		public void assign(int oID, int rID) { // Assign referee to offer
			dOffers[oID].assignedTo = rID;
			dReferees[rID].assignedTo.Add(oID);
			updateAvailability(true, oID, rID);
			currOffersFilled++;
			currPriorityFilled += dOffers[oID].actualOffer.priority;
			hFilledOffers.Add(oID);
		}

		long calcTabu(int oID) {
			return (initTemp / ((initTemp - currTemp) / ((long)dOffers[oID].available.Count() + (long)dOffers[oID].unavailable.Count())));
		}

		void setTabu(int oID, int rID) {
			int amount = (int)calcTabu(oID);
			dOffers[oID].available[rID].tabu += amount;
			dReferees[rID].available[oID].tabu += amount;
		}

		void unassign(int oID) {
			int rID = dOffers[oID].assignedTo;
			hFilledOffers.Remove(oID);
			dOffers[oID].assignedTo = -1;
			dReferees[rID].assignedTo.Remove(oID);
			updateAvailability(false, oID, rID);
			setTabu(oID, rID);
			currOffersFilled--;
			currPriorityFilled -= dOffers[oID].actualOffer.priority;
		}

		int randomAvailableRef(int oID) { // get a random available Ref  TODO: MAKE IT PICK ONE WITH TABU IF NONE AVAILABLE
			Random rand = new Random();
			foreach (var i in UniqueRandomValues(dOffers[oID].available)) {
				if (checkTabu(oID, i)) {
					return i;
				}
			}
			return dOffers[oID].available.ElementAt(rand.Next(0, dOffers[oID].available.Count)).Key; //if all have tabu, assign random
		}

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

		long calcOffersToReset() {
			if (dOffers.Count() < 20)
				return 1;
			return (long)(((0.2 * dOffers.Count()) * (initTemp / currTemp)) + 1);
		}

		void unassignRandom() {
			Random rand = new Random();
			unassign(hFilledOffers.ElementAt(rand.Next(0, hFilledOffers.Count())));
		}

		void saveState() {
			if (bestOffers.Count > 2) { // if there are 3 elements remove one before inserting
				bestOffers.RemoveAt(rand.Next(0, 3));
			}
			bestOffers.Add(new Dictionary<int, offerInfo>(dOffers));
			bestReferees.Add(new Dictionary<int, refInfo>(dReferees));
		}

		void resetState() {
			Random rand = new Random();
			int chooseState = rand.Next(0, bestReferees.Count());
			dOffers = new Dictionary<int, offerInfo>(bestOffers[chooseState]);
			dReferees = new Dictionary<int, refInfo>(bestReferees[chooseState]);
		}

		bool SimulatedAnnealing(int dif) {
			double difPercent = dif / maxPriorityFilled;
			double currTempPercent = currTemp / initTemp;
			if (rand.Next(0, 101) > ((currTempPercent + difPercent) * 100))
				return true; //accept
			else
				return false; //deny

		}

		void performAlgorithm() {
			for (currTemp = 0; currTemp < initTemp && bestOffersFilled < maxOffersFilled; currTemp++) {
				long countAssign = calcOffersToReset();
				for (long i = 0; i < countAssign; i++) {
					unassignRandom();
				}
				while (hCanBeFilledOffers.Count() > 0) {
					int currOffer = hCanBeFilledOffers.ElementAt(rand.Next(0, hCanBeFilledOffers.Count()));
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

		void setModel() {
			modelResult = new AlgorithmModel(1);
			foreach (var i in llCompleted) {
				modelResult.result[0].pairs.Add(new Models.pair(i.offer, i.referee, db));
			}
		}

		void saveResults() {
			if (bestOffers != null) {
				foreach (var i in bestOffers.First()) {
					llCompleted.AddLast(new pair(i.Key,i.Value.assignedTo));
				}
			}
		}

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
