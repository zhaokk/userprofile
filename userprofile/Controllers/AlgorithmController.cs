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

		public ActionResult showResults() {
			AssignReferees();
			modelResult.sortResult();
			return View(modelResult);
		}
        [HttpPost]
        public ActionResult showResults(Boolean assignAll, System.DateTime startDate, System.DateTime endDate) {
			dateStart = startDate;
			dateEnd = endDate;

            AssignReferees();
            modelResult.sortResult();
            return View(modelResult);
        }

		public ActionResult Index() {

			return View();
		}

		class pair {
			public int referee; //referee
			public int offer; //offer

			public pair(int off, int refs) {
				referee = refs;
				offer = off;
			}
		}
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

		class Item {
			public int tabu; //At what temperature this item will not be tabu anymore

			public Item() {
				tabu = -1;
			}
		}

		Dictionary<int, offerInfo> dOffers;
		Dictionary<int, refInfo> dReferees; //offers, referees
		LinkedList<pair> llCompleted; //Best options for offers/referees
		HashSet<int> hFilledOffers, hCanBeFilledOffers;
		Dictionary<int, HashSet<int>> dQualificationRefStorage;
		Dictionary<int, OFFER> dOfferStorage;
		Dictionary<int, REFEREE> dRefereeStorage;
		Dictionary<int, MATCH> dMatchStorage;
		List<Dictionary<int, offerInfo>> bestOffers;
		List<Dictionary<int, refInfo>> bestReferees;
		Dictionary<int, Dictionary<int, bool>> matchClashes;
		Dictionary<int, int> dGamesAvailableToRef;
		DateTime dateCurrent, dateStart, dateEnd;


		Raoconnection db;
		AlgorithmModel modelResult;
		int maxOffersFilled; //Max amount of offers that can be filled
		int currOffersFilled; //current number of offers filled
		long initTemp; //How many iterations of search to go through
		long currTemp; //What current iteration (temperature it is)
		int bestOffersFilled;
		Random rand;
		int currPriorityFilled;
		int bestPriorityFilled;
		int maxPriorityFilled;

		void initGlobalVars() {
			db = new Raoconnection();
			rand = new Random();
			dOffers = new Dictionary<int, offerInfo>();
			dReferees = new Dictionary<int, refInfo>();
			llCompleted = new LinkedList<pair>();
			hFilledOffers = new HashSet<int>();
			hCanBeFilledOffers = new HashSet<int>();
			dQualificationRefStorage = new Dictionary<int, HashSet<int>>();
			dOfferStorage = new Dictionary<int, OFFER>();
			dRefereeStorage = new Dictionary<int, REFEREE>();
			dMatchStorage = new Dictionary<int, MATCH>();
			matchClashes = new Dictionary<int, Dictionary<int, bool>>();
			dGamesAvailableToRef = new Dictionary<int, int>();

			bestOffersFilled = 0;
			maxOffersFilled = 0;
			currOffersFilled = 0;
			initTemp = 1000;
			currPriorityFilled = 0;
			maxPriorityFilled = 0;
			bestPriorityFilled = 0;
		}

		bool checkTabu(int oID, int rID) { //check if this is tabu
			if (dOffers[oID].available[rID].tabu > 0) {
				dOffers[oID].available[rID].tabu -= 1;
				dReferees[oID].available[oID].tabu -= 1;
				return true;
			}
			else
				return false;
		}

		bool checkAvailable(int oID, int rID) { //check if this ref is available to referee this offer (mostly for conflicts timewise)
			if (dReferees[rID].canAssign == dReferees[rID].assignedTo.Count()) { //referee is assigned to max
				return false;
			}
			if (dOffers[oID].assignedTo != -1) {//already filled
				return false;
			}
			foreach (var i in dReferees[rID].assignedTo) {
				if (dOfferStorage[i].matchId == dOfferStorage[oID].matchId) //if offers are on the same match
					return false;
				if (!matchClashes.ContainsKey(dOfferStorage[i].matchId)) {
					calculateClash(dOfferStorage[oID].matchId, dOfferStorage[i].matchId);
				}
				else if (!matchClashes[dOfferStorage[i].matchId].ContainsKey(dOfferStorage[oID].matchId)) {
					calculateClash(dOfferStorage[oID].matchId, dOfferStorage[i].matchId);
				}
				if (matchClashes[dOfferStorage[i].matchId][dOfferStorage[oID].matchId] == true) {
					return false;
				}
			}
			return true;
		}

		void setPairUnavailable(int oID, int rID) { //make combination of Offer/Ref unavailable
			bool canBeFilled = (dOffers[oID].available.Count() > 0);
			if (!dOffers[oID].available.ContainsKey(rID)) {
				return; //ALREADY REMOVED -> Most common cause is when it is the offer you assigned
			}
			dOffers[oID].unavailable.Add(rID, dOffers[oID].available[rID]);
			dOffers[oID].available.Remove(rID);
			dReferees[rID].unavailable.Add(oID, dReferees[rID].available[oID]);
			dReferees[rID].available.Remove(oID);
			if (canBeFilled) {
				if (dOffers[oID].available.Count() == 0) {
					hCanBeFilledOffers.Remove(oID);
				}
			}
		}

		void setPairAvailable(int oID, int rID) {
			bool canBeFilled = (dOffers[oID].available.Count() > 0);
			dOffers[oID].available.Add(rID, dOffers[oID].unavailable[rID]);
			dOffers[oID].unavailable.Remove(rID);
			dReferees[rID].available.Add(oID, dReferees[rID].unavailable[oID]);
			dReferees[rID].unavailable.Remove(oID);
			if (!canBeFilled) {
				if (dOffers[oID].available.Count() > 0) {
					hCanBeFilledOffers.Add(oID);
				}
			}
		}

		void updateAvailability(bool addRemove, int oID, int rID) { //Update availability of referee
			if (addRemove) { //if assigning a ref to an offer
				setOfferUnavailable(oID);
				if (dReferees[rID].canAssign == dReferees[rID].assignedTo.Count()) { //if assigned to max amount of offers that ref can have
					setRefereeUnavailable(rID);
					return;
				}
				LinkedList<int> nuke = new LinkedList<int>();
				foreach (var i in dReferees[rID].available) {
					if (!checkAvailable(i.Key, rID)) {
						nuke.AddLast(i.Key);//i.Key = oID
					}
				}
				foreach (var i in nuke) {
					setPairUnavailable(i, rID);
				}
			}
			else {
				LinkedList<int> nuke = new LinkedList<int>();
				foreach (var i in dOffers[oID].unavailable) {
					if (checkAvailable(oID, i.Key)) {
						nuke.AddLast(i.Key);
					}
				}
				foreach (var i in nuke) {
					setPairAvailable(oID, i);
				}
				nuke = new LinkedList<int>();
				foreach (var i in dReferees[rID].unavailable) {
					if (checkAvailable(i.Key, rID)) {
						nuke.AddLast(i.Key);
					}
				}
				foreach (var i in nuke) {
					setPairAvailable(i, rID);
				}
			}
		}

		void setRefereeUnavailable(int rID) { //Referee has been assigned to canAssign offers
			LinkedList<int> nuke = new LinkedList<int>();
			foreach (var i in dReferees[rID].available) {
				nuke.AddLast(i.Key);
			}
			foreach (var i in nuke) {
				setPairUnavailable(i, rID);
			}
		}

		void setOfferUnavailable(int oID) { //Offer has been assigned a referee
			LinkedList<int> nuke = new LinkedList<int>();
			foreach (var i in dOffers[oID].available) {
				nuke.AddLast(i.Key);
			}
			foreach (var i in nuke) {
				setPairUnavailable(oID, i);
			}
		}

		void getOffersAndQualificationIDs() {
			foreach (var i in db.OFFERs) {
				if (i.status == 5) {
					dOfferStorage.Add(i.offerId, i); //store offer by offerID
					foreach (var k in i.OFFERQUALs) {
						if (!dQualificationRefStorage.ContainsKey(k.qualificationId))
							dQualificationRefStorage.Add(k.qualificationId, new HashSet<int>()); //Make list of qualifications needed by all offers
					}
					dOffers.Add(i.offerId, new offerInfo(i)); //store offer ID for use of main algorithm
					maxPriorityFilled += i.priority;
				}
			}
		}

		int countOffersAlreadyAssigned(int rID) {
			//return db.OFFERs.Where(o => (o.REFEREE.refId == rID) && DateTime.Compare(o.MATCH.matchDate, dateCurrent) >= 0 && DateTime.Compare(o.MATCH.matchDate, dateCurrent.AddDays(1)) <= 0).Count();
			return db.OFFERs.Where(o => o.refId == rID && (o.status == 1 || o.status == 3) && o.dateOfOffer == dateCurrent).Count();
		}

		void fillRefereeByQualification() {
			foreach (var i in dQualificationRefStorage) { //make all qualificationRefStorage[qID] have all referee id's with qualification
				foreach (var j in db.REFEREEs) {
					foreach (var k in j.USERQUALs) {
						if (i.Key == k.qualificationId) {
							if (!dRefereeStorage.ContainsKey(j.refId)) {
								dRefereeStorage.Add(j.refId, j);
								dGamesAvailableToRef.Add(j.refId, j.maxGames - countOffersAlreadyAssigned(j.refId));
							}
							if (dGamesAvailableToRef[j.refId] > 0)
								i.Value.Add(j.refId); //adding refID to qualificationRefStorage[qID]

						}
					}
				}
			}
		}

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
			if (bestOffers != null) {
				modelResult = new AlgorithmModel(bestOffers.Count);//bestOffers.Count()
				for (int i = 0; i < bestOffers.Count; i++) {//bestOffers.Count()
					foreach (var j in bestOffers[i]) {
						modelResult.result[i].pairs.Add(new Models.pair(j.Key, j.Value.assignedTo, db));
					}
					foreach (var j in llCompleted) {
						modelResult.result[i].pairs.Add(new Models.pair(j.offer, j.referee, db));
					}
				}
			}
			else {
				modelResult = new AlgorithmModel(1);
				foreach (var i in llCompleted) {
					modelResult.result[0].pairs.Add(new Models.pair(i.offer, i.referee, db));
				}
			}
		}

		public void AssignReferees() {
			for (dateCurrent = dateStart; dateCurrent <= dateEnd; dateCurrent = dateCurrent.AddDays(1)) {
				initGlobalVars();
				fillSets();
				initChecks();
				performAlgorithm();
				setModel();
			}


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
