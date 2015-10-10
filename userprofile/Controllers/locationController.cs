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
    public class locationController : Controller
    {
        private Raoconnection db = new Raoconnection();
       
        // GET: /location/
        public ActionResult Index()
        {
            return View(db.LOCATIONs.ToList());
        }

        // GET: /location/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOCATION location = db.LOCATIONs.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // GET: /location/Create
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: /location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Create([Bind(Include="locationId,name,price,street,city,postcode,country,phoneNum,state")] LOCATION location)
        {
            Boolean shouldFail = false;
            if (ModelState.IsValid)
            {

                //check that the name + address is unique
                if (!checkUnique(location.geogCol2, location.name))
                {
                    ModelState.AddModelError("Location", "this location alredy exists");
                    shouldFail = true;
                }
                if (!shouldFail)
                {
                    //location.country = "Australia"; //### should not hard code here, edit later on
                    location.status = 1;
                    db.LOCATIONs.Add(location);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }

            return View(location);
        }



        // GET: /location/Edit/5
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOCATION location = db.LOCATIONs.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: /location/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Edit([Bind(Include = "locationId,name,price,street,city,country,postcode,phoneNum,state")] LOCATION location)
        {
            Boolean shouldFail = false;
            if (ModelState.IsValid)
            {
                var loc = db.LOCATIONs.Find(location.locationId);
                //check to see if we've kept the old name or address
                if (loc.name != location.name || loc.geogCol2 != location.geogCol2)
                {
                    //if the old is not being used, then it should not exist, so check that's the case
                    if (!checkUnique(location.geogCol2, location.name))
                    {
                        ModelState.AddModelError("Location", "this location alredy exists");
                        shouldFail = true;
                    }
                }

                 if (!shouldFail)
                 {
                     location.country = "Australia";
                     db.Entry(location).State = EntityState.Modified;
                     db.SaveChanges();
                     return RedirectToAction("Index");
                 }
            }
            return View(location);
        }

        private Boolean checkUnique(String location, String name)
        {

            var db = new Raoconnection();

            if (db.LOCATIONs.Any(user => user.name == name && user.geogCol2 == location))
            {
                return false; //found it, so it is not unique
            }
            return true; //didnt fint it, so it's unique
        }

        // GET: /location/Delete/5
        [Authorize(Roles = "Admin,Organizer")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LOCATION location = db.LOCATIONs.Find(id);
            if (location == null)
            {
                return HttpNotFound();
            }
            return View(location);
        }

        // POST: /location/Delete/5
        [Authorize(Roles = "Admin,Organizer")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LOCATION location = db.LOCATIONs.Find(id);
            db.LOCATIONs.Remove(location);
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
