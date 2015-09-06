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
    public class AlgorithmController : Controller
    {

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

		class val {
			public Dictionary<int, Item> available, unavailable; //available & unavailable referees/offers of that offer/referee
			public HashSet<int> assignedTo; //what that offer/referee is assigned to (offer is always only 1)
			public int canAssign; //How many offers the referee can be assigned to (offers will always be 1)

			public val(int num) {
				available = new Dictionary<int, Item>();
				unavailable = new Dictionary<int, Item>();
				assignedTo = new HashSet<int>();
				canAssign = num;
			}
		}

		class Item {
			public int tabu; //At what temperature this item will not be tabu anymore
			
			public Item() {
				tabu = -1;
			}
		}

		Dictionary<int, val> dOffers, dReferees; //offers, referees
		LinkedList<pair> llCompleted; //Best options for offers/referees
		HashSet<int> hFilledOffers, hCanBeFilledOffers;
        Dictionary<int, HashSet<int>> qualificationRefStorage;
        Dictionary<int, OFFER> offerStorage;
        Dictionary<int, REFEREE> refereeStorage;
        List<Dictionary<int, val>> bestOffers, bestReferees;
		Dictionary<int, Dictionary<int, bool>> matchClashes;

        Entities db;
		int maxOffersFilled; //Max amount of offers that can be filled
		int currOffersFilled; //current number of offers filled
		long initTemp; //How many iterations of search to go through
		long currTemp; //What current iteration (temperature it is
		int bestOffersFilled;
       

        void initGlobalVars() {
            db = new Entities();
            dOffers = new Dictionary<int, val>();
            dReferees = new Dictionary<int, val>();
            llCompleted = new LinkedList<pair>();
            hFilledOffers = new HashSet<int>();
            hCanBeFilledOffers = new HashSet<int>();
            qualificationRefStorage = new Dictionary<int, HashSet<int>>();
            offerStorage = new Dictionary<int, OFFER>();
            refereeStorage = new Dictionary<int, REFEREE>();

            maxOffersFilled = 0;
            currOffersFilled = 0;
            initTemp = 1000;
        }
		bool checkTabu(int oID, int rID) { //check if this is tabu
			return false;
		}

		bool checkAvailable(int oID, int rID) { //check if this ref is available to referee this offer (mostly for conflicts timewise)
			return true;
		}

		void setPairUnavailable(int oID, int rID) { //make combination of Offer/Ref unavailable
			bool canBeFilled = (dOffers[oID].available.Count() > 0);
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
				foreach (var i in dReferees[rID].available) {
					if (!checkAvailable(i.Key, rID)) {
						setPairUnavailable(oID, rID);
					}
				}
			}
			else {
				foreach (var i in dOffers[oID].unavailable) {
					if (checkAvailable(oID, i.Key)) {
						setPairAvailable(oID, i.Key);
					}
				}
				foreach (var i in dReferees[rID].unavailable) {
					if (checkAvailable(i.Key, rID)) {
						setPairAvailable(i.Key, rID);
					}
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
                if (i.status == 4) {
                    offerStorage.Add(i.offerID, i); //store offer by offerID
                    foreach (var k in i.QUALIFICATIONS) {
                        qualificationRefStorage.Add(k.qID, new HashSet<int>()); //Make list of qualifications needed by all offers
                    }
                    dOffers.Add(i.offerID, new val(1)); //store offer ID for use of main algorithm
                }
            }
        }

        void fillRefereeByQualification() {
            foreach (var i in qualificationRefStorage) { //make all qualificationRefStorage[qID] have all referee id's with qualification
                foreach (var j in db.REFEREEs) {
                    foreach (var k in j.QUALIFICATIONS) {
                        if (i.Key == k.qID) { //check level here later on
                            i.Value.Add(j.refID); //adding refID to qualificationRefStorage[qID]
                            if (!refereeStorage.ContainsKey(j.refID)) {
                                refereeStorage.Add(j.refID, j);
                            }
                        }
                    }
                }
            }
        }

        bool checkInitAvailability(int oID, int rID) {
            return true;
        }

        void addInitialRefereesToOffer(int oID, int rID) {
            dOffers[oID].available.Add(rID, new Item());
            if (!dReferees.ContainsKey(rID)) {
                dReferees.Add(rID, new val(refereeStorage[rID].maxGames)); //needs to add max games they can play here
                maxOffersFilled += dReferees[rID].canAssign; //count what top number of offers to fill is
            }
            dReferees[rID].available.Add(oID, new Item());
        }

        void addRefereesToOffers() {
            foreach (var i in offerStorage) {
                HashSet<int> availableReferees = null;
                foreach (var j in i.Value.QUALIFICATIONS) {
                    if (availableReferees == null) {
                        availableReferees = new HashSet<int>();
                        foreach (var k in qualificationRefStorage[j.qID]) {
                            if (checkInitAvailability(i.Key, k)) {

                            }
                        }
                    }
                    else {
                        HashSet<int> temp = availableReferees;
                        availableReferees = new HashSet<int>();
                        foreach (var k in temp) {
                            if (qualificationRefStorage[j.qID].Contains(k)) {
                                availableReferees.Add(k);
                            }
                        }
                    }
                    if (availableReferees.Count() == 0) {
                        break;
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

		bool calcMatchTimeClash(int i, int j) {
			
			return true;
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
						matchClashes.Add(j,new Dictionary<int,bool>());
					}
					matchClashes[j].Add(i,tmp);
				}
			}
			else {
				bool tmp = calcMatchTimeClash(i,j);
				matchClashes.Add(i,new Dictionary<int,bool>());
				matchClashes[i].Add(j,tmp);
				if (!matchClashes.ContainsKey(j)) {
					matchClashes.Add(j,new Dictionary<int,bool>());
				}
				matchClashes[j].Add(i,tmp);
			}
		}

		void calculateClashes() {
			foreach (var i in dReferees) {
				foreach (var j in i.Value.available) {
					foreach (var k in i.Value.available) {
						if (j.Key != k.Key) {
							calculateClash(j.Key, k.Key);
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
			foreach (var i in dOffers[oID].available) {
				dReferees[i.Key].available.Remove(oID);
				if (dReferees[i.Key].available.Count() == 0) {
					maxOffersFilled -= dReferees[i.Key].canAssign;
					dReferees.Remove(i.Key);
				}
			}
			dOffers.Remove(oID);
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
			dOffers[oID].assignedTo.Add(rID);
			dReferees[rID].assignedTo.Add(oID);
			updateAvailability(true, oID, rID);
			currOffersFilled++;
			hFilledOffers.Add(oID);
		}

		void unassign(int oID) {
			//SET TABU HERE
			int rID = dOffers[oID].assignedTo.First();
			hFilledOffers.Remove(oID);
			dOffers[oID].assignedTo.Remove(rID);
			dReferees[rID].assignedTo.Remove(oID);
			updateAvailability(false, oID, rID);
			currOffersFilled--;
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
							llCompleted.AddLast(new pair(k.Key, i.Key)); //mark as completed
							nuke.AddLast(k.Key); //mark to remove the offer
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
		}

		long calcOffersToReset() {
			if (dOffers.Count() < 20)
				return 1;
			return (long)(((0.2 * dOffers.Count()) * (initTemp / currTemp)) + 1);
		}

		void unassignRandom() {
			Random rand = new Random();
			unassign(rand.Next(0, hFilledOffers.Count()));
		}

		void saveState() {
			bestOffers.Add(new Dictionary<int, val>(dOffers));
			bestReferees.Add(new Dictionary<int, val>(dReferees));
		}

		void resetState() {
			Random rand = new Random();
			int chooseState = rand.Next(0, bestReferees.Count());
			dOffers = new Dictionary<int, val>(bestOffers[chooseState]);
			dReferees = new Dictionary<int, val>(bestReferees[chooseState]);
		}

		void performAlgorithm() {
			Random rand = new Random();
			for (currTemp = initTemp; currTemp > 0 && currOffersFilled < maxOffersFilled; currTemp--) {
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
					bestOffers.Clear();
					bestReferees.Clear();
					saveState();
				}
				else if (currOffersFilled == bestOffersFilled) {
					saveState();
				}
				else { //worse than best
					//if sim annealing stuff
					resetState();
				}
			}
		}
		
		public void AssignReferees() {
            initGlobalVars();
			fillSets();
			initChecks();
			performAlgorithm();
            //db.Entry(of).State = EntityState.Modified;
            //db.SaveChanges();
		}
	}
}