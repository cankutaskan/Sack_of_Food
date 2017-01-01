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
    public class FoodsController : Controller
    {
        private SofModel db = new SofModel();

        // GET: Foods
        public ActionResult FoodIndex()
        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

            var id = new SofModel().Restaurants
                  .Where(u => u.UserID == userID)
                  .Select(u => u.RestaurantID).FirstOrDefault();

            var foods = db.Foods.Where(f => f.RestaurantID==id);
            return View(foods.ToList());
        }



        // GET: Foods/Create
        public ActionResult FoodCreate()
        {
            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name");
            return View();
        }

        // POST: Foods/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
  
        public ActionResult FoodCreate([Bind(Include = "FoodID,Name,Price,Description,FoodStatu")] Foods foods)
        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

            var id = new SofModel().Restaurants
                  .Where(u => u.UserID == userID)
                  .Select(u => u.RestaurantID).FirstOrDefault();
            if (ModelState.IsValid)
            {
                foods.RestaurantID = id;
                db.Foods.Add(foods);
                db.SaveChanges();
                return RedirectToAction("FoodIndex");
            }

            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", foods.RestaurantID);
            return View(foods);
        }

        // GET: Foods/Edit/5
        public ActionResult FoodEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Foods foods = db.Foods.Find(id);
            if (foods == null)
            {
                return HttpNotFound();
            }
            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", foods.RestaurantID);
            return View(foods);
        }

        // POST: Foods/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
     
        public ActionResult FoodEdit([Bind(Include = "FoodID,Name,Price,Description,RestaurantID,FoodStatu")] Foods foods)
        {
            if (ModelState.IsValid)
            {
              
                

               Foods orijinalFood = SOFEntity.getDb().Foods.Find(foods.FoodID);
               SOFEntity.getDb().Entry(orijinalFood).CurrentValues.SetValues(foods);
               
            //    db.Entry(foods).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("FoodIndex");
            }
            ViewBag.RestaurantID = new SelectList(db.Restaurants, "RestaurantID", "Name", foods.RestaurantID);
            return View(foods);
        }

        // GET: Foods/Delete/5
        public ActionResult FoodDelete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Foods foods = db.Foods.Find(id);
            if (foods == null)
            {
                return HttpNotFound();
            }
            return View(foods);
        }

        // POST: Foods/Delete/5
        [HttpPost, ActionName("FoodDelete")]

        public ActionResult DeleteConfirmed(int id)
        {
            Foods foods = db.Foods.Find(id);
            db.Foods.Remove(foods);
            db.SaveChanges();
            return RedirectToAction("FoodIndex");
        }

  
    }
}
