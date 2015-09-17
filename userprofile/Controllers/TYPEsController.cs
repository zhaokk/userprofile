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
    public class TYPEsController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: TYPEs
        public ActionResult Index()
        {
            return View(db.TYPEs.ToList());
        }

        // GET: TYPEs/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TYPE tYPE = db.TYPEs.Find(id);
            if (tYPE == null)
            {
                return HttpNotFound();
            }
            return View(tYPE);
        }

        // GET: TYPEs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: TYPEs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "typeId,name,description,priority,status")] TYPE tYPE)
        {
            if (ModelState.IsValid)
            {
                db.TYPEs.Add(tYPE);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(tYPE);
        }

        // GET: TYPEs/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TYPE tYPE = db.TYPEs.Find(id);
            if (tYPE == null)
            {
                return HttpNotFound();
            }
            return View(tYPE);
        }

        // POST: TYPEs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "typeId,name,description,priority,status")] TYPE tYPE)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tYPE).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(tYPE);
        }

        // GET: TYPEs/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TYPE tYPE = db.TYPEs.Find(id);
            if (tYPE == null)
            {
                return HttpNotFound();
            }
            return View(tYPE);
        }

        // POST: TYPEs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            TYPE tYPE = db.TYPEs.Find(id);
            db.TYPEs.Remove(tYPE);
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
