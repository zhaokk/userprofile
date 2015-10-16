﻿using System;
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
    public class TournamentController : Controller
    {
        private Raoconnection db = new Raoconnection();
         [HttpPost]
        public string  TournamentSelected(string tournamentID)
        {
            if (tournamentID == "")
            {
                Session["tournamentID"] = null;
                Session["tournamentname"] = null;
            }
            else { 
                Session["tournamentID"] = tournamentID;
                int tid = Int32.Parse(tournamentID);
            Session["tournamentname"] = db.TOURNAMENTs.Find(tid).name;
               }
           return ("tournamentID has change to"+ tournamentID);
        }
        // GET: TOURNAMENTs
        public async Task<ActionResult> Index()
        {
            ViewBag.session = Session["tournamentID"] as string;
            var tournament = db.TOURNAMENTs.Include(t => t.AspNetUser).Include(t => t.SPORT1).Where(tour => tour.status == 1);
            ViewBag.TournamentList = new SelectList(db.TOURNAMENTs, "tournamentId", "name", Session["tournamentID"]);
            
            return View(await tournament.ToListAsync());
        }
        public ActionResult History() {
            var oldTour = db.TOURNAMENTs.Where(d => d.status == 2).ToList();
            return View(oldTour);
        
        
        }

        // GET: TOURNAMENTs/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // GET: TOURNAMENTs/Create
        public ActionResult Create()
        {
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName");
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name");
            return View();
        }

        // POST: TOURNAMENTs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Create([Bind(Include = "tournamentId,name,startDate,organizer,ageBracket,grade,sport,status,priority")] TOURNAMENT tournament)
        {
            tournament.status = 1;
            if (ModelState.IsValid)
            {
                db.TOURNAMENTs.Add(tournament);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tournament.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tournament.sport);
            return View(tournament);
        }

        // GET: TOURNAMENTs/Edit/5
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tournament.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tournament.sport);
            return View(tournament);
        }

        // POST: TOURNAMENTs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Organizer")]
        public async Task<ActionResult> Edit([Bind(Include = "tournamentId,name,startDate,organizer,ageBracket,grade,sport,status,priority")] TOURNAMENT tournament)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tournament).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.organizer = new SelectList(db.AspNetUsers, "Id", "UserName", tournament.organizer);
            ViewBag.sport = new SelectList(db.SPORTs, "name", "name", tournament.sport);
            return View(tournament);
        }

        // GET: TOURNAMENTs/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            if (tournament == null)
            {
                return HttpNotFound();
            }
            return View(tournament);
        }

        // POST: TOURNAMENTs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            TOURNAMENT tournament = await db.TOURNAMENTs.FindAsync(id);
            

            tournament.status = 0;
            db.Entry(tournament).State = EntityState.Modified;
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
