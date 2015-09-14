using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;
using Microsoft.AspNet.Identity;

namespace userprofile.Controllers
{
    public class REFEREEController : Controller
    {
        private Entities db = new Entities();

        // GET: /REFEREE/
        public ActionResult Index()
        {
            var referees = db.REFEREEs.Include(r => r.AspNetUser).Include(r => r.SPORT1);
            return View(referees.ToList());
        }

        // GET: /REFEREE/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REFEREE referee = db.REFEREEs.Find(id);
            if (referee == null)
            {
                return HttpNotFound();
            }
            return View(referee);
        }

        //// GET: /REFEREE/Create
        //public ActionResult Create()
        //{
        //    ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName");
        //    ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
        //    ViewBag.qualifaction = new MultiSelectList(db.QUALIFICATIONS, "qID", "name");
        //    return View();
        //}

        //// POST: /REFEREE/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create( REFEREE referee)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.REFEREEs.Add(referee);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.ID = new SelectList(db.AspNetUsers, "re.Id", "UserName", referee.ID);
        //    ViewBag.sport = new SelectList(db.SPORTs, "name", "name", referee.sport);
        //    return View(referee);
        //}

        // GET: /REFEREE/Create
        public ActionResult Create()
        {
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            ViewBag.qualifaction = new MultiSelectList(db.QUALIFICATIONS, "qID", "name");
            ViewBag.quallist = new selectRefQuliViewModel(db);
            REFEREEqualViewModel rqvm = new REFEREEqualViewModel(db);
            return View(rqvm);
        }

        // POST: /REFEREE/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( selectRefQuliViewModel srqvm, REFEREE re)
        {
        


            if (ModelState.IsValid)
            {
                re.QUALIFICATIONS.Clear();
                foreach (var qual in srqvm.quals)
                {
                    QUALIFICATION thequal = db.QUALIFICATIONS.First(q => q.name == qual.qualName);

                    if (qual.Selected == true)
                    {
                        re.QUALIFICATIONS.Add(thequal);
                    }

                }
                db.REFEREEs.Add(re);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName", re.ID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", re.sport);
            REFEREEqualViewModel rqvm = new REFEREEqualViewModel(db);
            rqvm.re = re;
            rqvm.srqvm = srqvm;
            return View(rqvm);
        }
        public ActionResult Createe( REFEREE re)
        {



            if (ModelState.IsValid)
            {
               
                
                db.REFEREEs.Add(re);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

           
            return View();
        }
        public ActionResult changequal(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REFEREE referee = db.REFEREEs.Find(id);
            if (referee == null)
            {
                return HttpNotFound();
            }
            selectRefQuliEditViewModel selected = new selectRefQuliEditViewModel(referee, db);
            return View(selected);


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult changequal( selectRefQuliEditViewModel srqvm)
        {

          
            if (ModelState.IsValid)
            {
              REFEREE refe=  db.REFEREEs.First(r => r.refID == srqvm.refeid);
              refe.QUALIFICATIONS.Clear();
              foreach (SelectQualEditorViewModel qual in srqvm.quals)
              {
                  QUALIFICATION thequal = db.QUALIFICATIONS.First(q => q.name == qual.qualName);

                  if (qual.Selected == true)
                  {
                      refe.QUALIFICATIONS.Add(thequal);
                  }
              }
              db.Entry(refe).State = EntityState.Modified;
              db.SaveChanges();
                 
                return RedirectToAction("Index");

            }

            return View(srqvm);


        }
        // GET: /REFEREE/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REFEREE referee = db.REFEREEs.Find(id);
            if (referee == null)
            {
                return HttpNotFound();
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName", referee.ID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", referee.sport);
            return View(referee);
        }

        // POST: /REFEREE/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="refID,availability,distTravel,sport,prefAge,prefGrade,ID")] REFEREE referee)
        {
            if (ModelState.IsValid)
            {
                db.Entry(referee).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ID = new SelectList(db.AspNetUsers, "Id", "UserName", referee.ID);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", referee.sport);
            return View(referee);
        }

        // GET: /REFEREE/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            REFEREE referee = db.REFEREEs.Find(id);
            if (referee == null)
            {
                return HttpNotFound();
            }
            return View(referee);
        }

        // POST: /REFEREE/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            REFEREE referee = db.REFEREEs.Find(id);
            referee.QUALIFICATIONS.Clear();
            db.REFEREEs.Remove(referee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpGet]
        public ActionResult Availability() {
            var currentRefereeId= User.Identity.GetUserId();
            var refe= db.REFEREEs.First(rid=>rid.ID==currentRefereeId);
            if(refe.WEEKLYAVAILABILITY!=null){
           
            WEEKLYAVAILABILITYViewModel WAVM = new WEEKLYAVAILABILITYViewModel(refe.WEEKLYAVAILABILITY);

            return View(WAVM);}
            else return View(new WEEKLYAVAILABILITYViewModel());
        }
        [HttpPost]
        public ActionResult Availability(WEEKLYAVAILABILITYViewModel jsonData)
        {
            var currentRefereeId = User.Identity.GetUserId();
            var refe = db.REFEREEs.First(rid => rid.ID == currentRefereeId);
            if (refe.WEEKLYAVAILABILITY != null) { 
            db.WEEKLYAVAILABILITies.Remove(refe.WEEKLYAVAILABILITY);}
            refe.WEEKLYAVAILABILITY = jsonData.getWeekADb();
            db.Entry(refe).State = EntityState.Modified;
            db.SaveChanges();


            return View(jsonData);

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
