using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using SOF301.Models;
using System.Security.Claims;
using System.Data.Entity;
using System.Net;

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
            if (order != null)
            {


            }
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
            int userID = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);

            //  var user = SOFEntity.getDb().Users.Where(u => u.UserID == userID).ToList();
            var orders = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == userID && o.Orders.OrderStatus == null);
            //    var foods = SOFEntity.getDb().Foods.Where( )
            var price = SOFEntity.getDb().Foods.Join(orders,
              f => f.FoodID,
              o => o.FoodID,
              (f, o) => new { f.Price }).ToList();
            double totalPrice = 0;
            foreach (var item in price)
            {

                if (item != null)
                {
                    totalPrice += (double)(item.Price);

                }



            }
            var user = SOFEntity.getDb().Users.Find(userID);


            ViewBag.Address = user.Address;
            ViewBag.TotalPrice = totalPrice;


            return View();
        }

        [HttpPost]
        public ActionResult Payment([Bind(Include = "TotalPrice,Address,PaymentType,Description")] Orders orders)
        {
            
            int userID = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var order = SOFEntity.getDb().Orders.Where(o => o.UserID == userID && o.OrderStatus == null).FirstOrDefault();
            order.Description = orders.Description;
            order.PaymentType = orders.PaymentType;
            order.TotalPrice = orders.TotalPrice;
           // order.TotalPrice=
           
            if (orders.Address == null)
            {

                order.Address = SOFEntity.getDb().Users.Find(userID).Address;

            }
            else
            {
                order.Address = orders.Address;

            }


        
                order.OrderStatus = 1;
                var newOrder = new SOF301.Models.Orders();
                newOrder.UserID = userID;

               
                try
                {
                    SOFEntity.getDb().Orders.Add(newOrder);

                    SOFEntity.getDb().SaveChanges();
                }
                catch(Exception e)
                {



                }
                return RedirectToAction("Index");
            
            ViewBag.UserID = new SelectList(SOFEntity.getDb().Users, "UserID", "UserName", orders.UserID);
            return View();
        }
        
        public ActionResult ListOrders()
        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);


            var orders = SOFEntity.getDb().Orders.Where(o => o.UserID == userID);
            return View(orders.ToList());
        }
        
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var OrderID = SOFEntity.getDb().OrderItems.Find(id).OrderID;
            var list = SOFEntity.getDb().OrderItems.Where(o => o.OrderID == OrderID).ToList();

            return View(list);
        }
        
 

    }
}