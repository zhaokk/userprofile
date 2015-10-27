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
    public class qualificationController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: /qualification/
        public ActionResult Index()
        {
            var qualifications = db.QUALIFICATIONS.Include(q => q.SPORT1).Where(qual => qual.status > 0);
            return View(qualifications.ToList());
        }

        // GET: /qualification/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            var refs = db.USERQUALs.Where(a => a.qualificationId == qualification.qualificationId).Include(m => m.REFEREE).ToList();
            var reflist = new List<SelectListItem>();
            var refeWithoutQual = db.REFEREEs.ToList();
            var flag=true;
            foreach (REFEREE refe in db.REFEREEs)
            {
                 flag= true;
                foreach (var userqual in refs) { 
                if (refe.refId==userqual.refId)
                {
                    flag = false;
                   
                }
                }
                if (flag) {
                    var qitem = new SelectListItem();
                    qitem.Text = refe.AspNetUser.firstName + " " + refe.AspNetUser.lastName;
                    qitem.Value = refe.refId.ToString();
                    reflist.Add(qitem);
                }
             
            }
            ViewBag.refeList = reflist;
            if (qualification == null)
            {
                return HttpNotFound();
            }

            var combined = new Tuple<QUALIFICATION, List<USERQUAL>>(qualification, refs) { };
            return View(combined);
            //return View(qualification);
        }
        public ActionResult addReferee(int refereeID, int qlevel, int qualid) {
            USERQUAL userq = new USERQUAL()
            {
                refId = refereeID,
                qualificationId = qualid,
                qualLevel = qlevel

            };
            db.USERQUALs.Add(userq);
            db.SaveChanges();
            return RedirectToAction("Details", new { id = qualid });
        }
        // GET: /qualification/Create
        public ActionResult Create()
        {
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            return View();
        }

        // POST: /qualification/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "qualificationId,name,sport,description")] QUALIFICATION qualification)
        {
            if (ModelState.IsValid)
            {
                qualification.status = 1;
                db.QUALIFICATIONS.Add(qualification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", qualification.sport);
            return View(qualification);
        }

        // GET: /qualification/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            db.Entry(qualification).Reference(r => r.SPORT1).Load();
            if (qualification == null)
            {
                return HttpNotFound();
            }
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", qualification.sport);
            return View(qualification);
        }

        // POST: /qualification/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "qualificationId,name,sport,description,qualificationLevel")] QUALIFICATION qualification)
        {
            if (ModelState.IsValid)
            {
                //qualification.SPORT1 = db.SPORTs.Find(qualification.sport);
                db.Entry(qualification).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", qualification.sport);
            return View(qualification);
        }

        // GET: /qualification/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            if (qualification == null)
            {
                return HttpNotFound();
            }
            return View(qualification);
        }

        // POST: /qualification/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            QUALIFICATION qualification = db.QUALIFICATIONS.Find(id);
            db.QUALIFICATIONS.Remove(qualification);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        //delete user quilifiaction in detail page
        public void deleteUserqual(int refereeID,int qualid) {
           List<USERQUAL> deletedUserQual =db.USERQUALs.Where(uq => uq.refId == refereeID).Where(uq => uq.qualificationId == qualid).ToList();
           db.USERQUALs.Remove(deletedUserQual.First());
           db.SaveChanges();
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
