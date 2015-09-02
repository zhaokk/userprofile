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
        private Entities db = new Entities();
        private int i = 1;
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
        public ActionResult Create()
        {
            return View();
        }

        // POST: /location/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include="lID,name,price,street,snum,city,postcode,phoneNum,state")] LOCATION location)
        {
            if (ModelState.IsValid)
            {
                db.LOCATIONs.Add(location);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(location);
        }

        // GET: /location/Edit/5
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
        public ActionResult Edit([Bind(Include="lID,name,price,street,snum,city,postcode,phoneNum,state")] LOCATION location)
        {
            if (ModelState.IsValid)
            {
                db.Entry(location).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(location);
        }

        // GET: /location/Delete/5
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
