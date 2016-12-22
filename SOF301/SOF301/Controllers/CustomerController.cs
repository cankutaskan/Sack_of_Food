using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SOF301.Models;
using System.Security.Claims;
using System.Data.Entity;

namespace SOF301.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer



        public ActionResult Index(String sortOrder, string currentFilter, string searchString, int? page)
        {
            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);

            var districtID = SOFEntity.getDb().Users.Where(u => u.UserID == user).Select(r => r.DistrictID).FirstOrDefault();

            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "Name_desc" : "";
            ViewBag.CurrentSort = sortOrder;
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;

            }
            ViewBag.CurrentFilter = searchString;


            var restaurant = SOFEntity.getDb().Restaurants.Where(r => r.DistrictID == districtID);

            if (!String.IsNullOrEmpty(searchString))
            {


                restaurant = restaurant.Where(r => r.Name.Contains(searchString));


                


            }

            switch (sortOrder)
            {
                case "Name_desc":
                    restaurant = restaurant.OrderByDescending(s => s.Name);
                    break;

                default:
                    restaurant = restaurant.OrderBy(s => s.Name);
                    break;
            }
            int pageSize = 3;
            int pageNumber = (page ?? 1);
            return View(restaurant.ToPagedList(pageNumber, pageSize));
        }
        [HttpGet]
        public ActionResult RestaurantPage(int? RestaurantID)
        {
            ViewBag.Title = SOFEntity.getDb().Restaurants.Where(r => r.RestaurantID == RestaurantID).Select(r => r.Name).FirstOrDefault();

            var foods = SOFEntity.getDb().Foods.Where(r => r.RestaurantID == RestaurantID);
            return View(foods.ToList());

        }
        public ActionResult getBasket()
        {
            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var basket = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();

            return PartialView("ItemBasket", basket);
        }

        public ActionResult ItemBasket(int? FoodID)
        {
            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var order = SOFEntity.getDb().Orders.Where(o => o.UserID == user && o.OrderStatus == null).FirstOrDefault();
            var orderItem = new SOF301.Models.OrderItems();
            orderItem.FoodID = FoodID;
            orderItem.OrderID = order.OrderID;
            try
            {


                SOFEntity.getDb().OrderItems.Add(orderItem);
                SOFEntity.getDb().SaveChanges();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var basket = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();
            return PartialView(basket);
        }

        public ActionResult Payment()
        {

            return View();
        }

        [HttpPost]
        public ActionResult Payment([Bind(Include = "OrderID,UserID,Date,TotalPrice,Telephone,Address,PaymentType,Description,OrderStatus")] Orders orders)
        {

            if (ModelState.IsValid)
            {
                SOFEntity.getDb().Entry(orders).State = EntityState.Modified;
                SOFEntity.getDb().SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(SOFEntity.getDb().Users, "UserID", "UserName", orders.UserID);
            return View(orders);
        }


    }
}