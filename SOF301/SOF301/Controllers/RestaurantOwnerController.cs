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
    [Authorize(Roles = "2,1")]
    public class RestaurantOwnerController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        // GET: Restaurants/Details/5
        public ActionResult Details(int? id)
        {
          
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

            id = new SofModel().Restaurants
                 .Where(u => u.UserID==userID)
                 .Select(u => u.RestaurantID).FirstOrDefault();

         

          Restaurants res = SOFEntity.getDb().Restaurants.Find(id);

            return View(res);
        }
        
        // GET: Restaurants/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Restaurants restaurants = SOFEntity.getDb().Restaurants.Find(id);
            if (restaurants == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name", restaurants.CityID);
            ViewBag.DistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name", restaurants.DistrictID);
            ViewBag.UserID = new SelectList(SOFEntity.getDb().Users, "UserID", "UserName", restaurants.UserID);
            return View(restaurants);
        }
        
        [HttpPost]
        public ActionResult Edit([Bind(Include = "RestaurantID,Name,CityID,DistrictID,Address,UserID,StartingHour,FinishingHour,RestaurantStatu")] Restaurants restaurants)
        {
            if (ModelState.IsValid)
            {
                Restaurants originalRestaurant = SOFEntity.getDb().Restaurants.Find(restaurants.UserID);
                SOFEntity.getDb().Entry(originalRestaurant).CurrentValues.SetValues(restaurants);
                //SOFEntity.getDb().Entry(restaurants).State = EntityState.Modified;
                SOFEntity.getDb().SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.CityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name", restaurants.CityID);
            ViewBag.DistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name", restaurants.DistrictID);
            ViewBag.UserID = new SelectList(SOFEntity.getDb().Users, "UserID", "UserName", restaurants.UserID);
            return View(restaurants);
        }

   
        
    }
}
