using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using userprofile.Models;

namespace userprofile.Controllers
{
    public class PLAYERsController : Controller
    {
        private Raoconnection db = new Raoconnection();

        // GET: PLAYERs
        public async Task<ActionResult> Index()
        {
            var pLAYERs = db.PLAYERs.Include(p => p.AspNetUser).Include(p => p.TEAM);
            return View(await pLAYERs.ToListAsync());
        }

        // GET: PLAYERs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER pLAYER = await db.PLAYERs.FindAsync(id);
            if (pLAYER == null)
            {
                return HttpNotFound();
            }
            return View(pLAYER);
        }

        // GET: PLAYERs/Create
        public ActionResult Create()
        {
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name");
            return View();
        }

        // POST: PLAYERs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "teamId,userId,position,shirtNum,status")] PLAYER pLAYER)
        {
            if (ModelState.IsValid)
            {
                db.PLAYERs.Add(pLAYER);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", pLAYER.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", pLAYER.teamId);
            return View(pLAYER);
        }

        // GET: PLAYERs/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER pLAYER = await db.PLAYERs.FindAsync(id);
            if (pLAYER == null)
            {
                return HttpNotFound();
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", pLAYER.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", pLAYER.teamId);
            return View(pLAYER);
        }

        // POST: PLAYERs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "teamId,userId,position,shirtNum,status")] PLAYER pLAYER)
        {
            if (ModelState.IsValid)
            {
                db.Entry(pLAYER).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.userId = new SelectList(db.AspNetUsers, "Id", "UserName", pLAYER.userId);
            ViewBag.teamId = new SelectList(db.TEAMs, "teamId", "name", pLAYER.teamId);
            return View(pLAYER);
        }

        // GET: PLAYERs/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PLAYER pLAYER = await db.PLAYERs.FindAsync(id);
            if (pLAYER == null)
            {
                return HttpNotFound();
            }
            return View(pLAYER);
        }

        // POST: PLAYERs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            PLAYER pLAYER = await db.PLAYERs.FindAsync(id);
            db.PLAYERs.Remove(pLAYER);
            await db.SaveChangesAsync();
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
