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
                SOFEntity.getDb().Entry(restaurants).State = EntityState.Modified;
                SOFEntity.getDb().SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.CityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name", restaurants.CityID);
            ViewBag.DistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name", restaurants.DistrictID);
            ViewBag.UserID = new SelectList(SOFEntity.getDb().Users, "UserID", "UserName", restaurants.UserID);
            return View(restaurants);
        }

        public ActionResult Orders()
        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

           int id = new SofModel().Restaurants
                .Where(u => u.UserID == userID)
                .Select(u => u.RestaurantID).FirstOrDefault();


            var orders = SOFEntity.getDb().Orders.Where(o => o.RestaurantID == id && o.OrderStatus == 0);
            return View(orders.ToList());
        }

        public ActionResult OrderDetails(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   var OrderID = SOFEntity.getDb().OrderItems.Find(id).OrderID;
            var list = SOFEntity.getDb().OrderItems.Where(o => o.OrderID == id).ToList();

            return View(list);
        }

        public ActionResult AcceptOrder(int id)
        {
            if (id != null)
            {
                Orders or = SOFEntity.getDb().Orders.Find(id);


                Orders updatedOr = or;
                updatedOr.OrderStatus = 1;

                SOFEntity.getDb().Entry(or).CurrentValues.SetValues(updatedOr);
                SOFEntity.getDb().SaveChanges();


            }
            return RedirectToAction("Orders");
        }

   
        
    }
}
