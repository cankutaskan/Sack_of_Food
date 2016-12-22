using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SOF301.Models;
using System.Security.Claims;

namespace SOF301.Controllers
{
    public class RestaurantsController : Controller
    {
        private SofModel db = new SofModel();



        // GET: Restaurants/Details/5
        public ActionResult Details(int? id)
        {
          
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

            id = new SofModel().Restaurants
                 .Where(u => u.UserID==userID)
                 .Select(u => u.RestaurantID).FirstOrDefault();

         

          Restaurants res = db.Restaurants.Find(id);

            return View(res);
        }

  



        // GET: Restaurants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurants restaurants = db.Restaurants.Find(id);
            if (restaurants == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", restaurants.CityID);
            ViewBag.DistrictID = new SelectList(db.Districts, "DistrictID", "Name", restaurants.DistrictID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", restaurants.UserID);
            return View(restaurants);
        }

        // POST: Restaurants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
      
        public ActionResult Edit([Bind(Include = "RestaurantID,Name,CityID,DistrictID,Address,UserID,StartingHour,FinishingHour,RestaurantStatu")] Restaurants restaurants)
        {
            if (ModelState.IsValid)
            {
                db.Entry(restaurants).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", restaurants.CityID);
            ViewBag.DistrictID = new SelectList(db.Districts, "DistrictID", "Name", restaurants.DistrictID);
            ViewBag.UserID = new SelectList(db.Users, "UserID", "UserName", restaurants.UserID);
            return View(restaurants);
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
