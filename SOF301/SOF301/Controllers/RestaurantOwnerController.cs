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
using SOF301.Tools;

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

        [HttpPost]
        public JsonResult GetDistrict(string id)
        {
            //   List<SelectListItem> districts = new List<SelectListItem>();

            int ID = int.Parse(id);
            var districts = SOFEntity.getDb().Districts.Where(d => d.CityID == ID);
            //  List<SelectList> district = new List(districts, "DistrictID", "Name");
            List<SelectListItem> ls = new List<SelectListItem>();

            foreach (var temp in districts)
            {

                ls.Add(new SelectListItem() { Text = temp.Name, Value = temp.DistrictID.ToString() });
            }
            return Json(new SelectList(ls, "Value", "Text"));
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

        public ActionResult CancelOrder(int id)
        {
            if (id != null)
            {
                Orders or = SOFEntity.getDb().Orders.Find(id);


                Orders updatedOr = or;
                updatedOr.OrderStatus = 2;

                SOFEntity.getDb().Entry(or).CurrentValues.SetValues(updatedOr);
                SOFEntity.getDb().SaveChanges();


            }
            return RedirectToAction("Orders");
        }

        // GET: Foods
        public ActionResult FoodIndex()
        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

            var id = new SofModel().Restaurants
                  .Where(u => u.UserID == userID)
                  .Select(u => u.RestaurantID).FirstOrDefault();

            var foods = SOFEntity.getDb().Foods.Where(f => f.RestaurantID == id);
            return View(foods.ToList());
        }


        
        // GET: Foods/Create
        public ActionResult FoodCreate()
        {
            ViewBag.RestaurantID = new SelectList(SOFEntity.getDb().Restaurants, "RestaurantID", "Name");
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
                SOFEntity.getDb().Foods.Add(foods);
                SOFEntity.getDb().SaveChanges();
                return RedirectToAction("FoodIndex");
            }

            ViewBag.RestaurantID = new SelectList(SOFEntity.getDb().Restaurants, "RestaurantID", "Name", foods.RestaurantID);
            return View(foods);
        }

        // GET: Foods/Edit/5
        public ActionResult FoodEdit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Foods foods = SOFEntity.getDb().Foods.Find(id);
            if (foods == null)
            {
                return HttpNotFound();
            }
            ViewBag.RestaurantID = new SelectList(SOFEntity.getDb().Restaurants, "RestaurantID", "Name", foods.RestaurantID);
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
                SOFEntity.getDb().SaveChanges();
                return RedirectToAction("FoodIndex");
            }
            ViewBag.RestaurantID = new SelectList(SOFEntity.getDb().Restaurants, "RestaurantID", "Name", foods.RestaurantID);
            return View(foods);
        }

        // GET: Foods/Delete/5
        public ActionResult FoodDelete(int? id)
        {
            if (id != null)
            {
                Foods food = SOFEntity.getDb().Foods.Find(id);


                
                SOFEntity.getDb().Foods.Remove(food);

              
                SOFEntity.getDb().SaveChanges();


            }
            return RedirectToAction("FoodIndex");
        }

  

   
        
    }
}
