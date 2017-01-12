
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
    [Authorize(Roles = "3,1")]
    public class CustomerController : Controller
    {


      public ActionResult Index(String sortOrder, string currentFilter, string searchString, int? page, string SearchBy, string CityID, string DistrictID)

        {



            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);

            //    var districtID = SOFEntity.getDb().Users.Where(u => u.UserID == user).Select(r => r.DistrictID).FirstOrDefault();

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
            System.Threading.Thread.Sleep(2000);
            var cityID = SOFEntity.getDb().Users.Where(u => u.UserID == user).FirstOrDefault().CityID;
            var restaurant = from res in SOFEntity.getDb().Restaurants
                             where res.CityID == cityID
                             select res;


            ViewBag.UserCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");

            ViewBag.UserDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");




            //   restaurant = restaurant.Where(r => r.Name.Contains(searchString));


            switch (SearchBy)
            {

                case "0":

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        restaurant = restaurant.Where(r => r.Name.Contains(searchString));
                    }



                    break;

                case "1":

                    int cID = int.Parse(CityID);


                    restaurant = restaurant.Where(r => r.CityID == cID);

                    break;

                case "2":

                    int dID = int.Parse(DistrictID);
                    restaurant = restaurant.Where(r => r.DistrictID == dID);


                    break;




                default:



                    break;




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
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem { Text = "Name", Value = "0", Selected = true });
            items.Add(new SelectListItem { Text = "City", Value = "1" });
            items.Add(new SelectListItem { Text = "District", Value = "2" });




            //   items.Add(new SelectListItem { Text = "Food", Value = "3" });

            ViewBag.Filter = items;

            int pageSize = 3;
            int pageNumber = (page ?? 1);

            return View(restaurant.ToPagedList(pageNumber, pageSize));
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
        
        
        [HttpGet]
        public ActionResult RestaurantPage(int? RestaurantID)
        {
            int userID = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            ViewBag.Title = SOFEntity.getDb().Restaurants.Where(r => r.RestaurantID == RestaurantID).Select(r => r.Name).FirstOrDefault();
            var order = SOFEntity.getDb().Orders.Where(o => o.UserID == userID && o.OrderStatus == null).FirstOrDefault();
            order.RestaurantID = RestaurantID;
            var foods = SOFEntity.getDb().Foods.Where(r => r.RestaurantID == RestaurantID);
            return View(foods.ToList());

        }

        public ActionResult getBasket()
        {
            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var basket = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();

            return PartialView("ItemBasket", basket);
        }

        public ActionResult ItemBasket(int? FoodID, int? amount)
        {
            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var order = SOFEntity.getDb().Orders.Where(o => o.UserID == user && o.OrderStatus == null).FirstOrDefault();
            if (order != null)
            {


            }
            for (int i = 0; i < amount; i++)
            {
                var orderItem = new SOF301.Models.OrderItems();
                orderItem.FoodID = FoodID;
                orderItem.OrderID = order.OrderID;

                SOFEntity.getDb().OrderItems.Add(orderItem);

            }



            //var orderItem = new SOF301.Models.OrderItems();
            //orderItem.FoodID = FoodID;
            //orderItem.OrderID = order.OrderID;
            try
            {



                SOFEntity.getDb().SaveChanges();
            }
            catch (Exception e)
            {
                ViewData["EditError"] = e.Message;
            }
            var basket = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();
            return PartialView(basket);
        }

        public ActionResult removeBasket(int FoodID)
        {
            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var temp = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();
            var orderItem = temp.Where(o => o.FoodID == FoodID).FirstOrDefault();


            try
            {
                SOFEntity.getDb().OrderItems.Remove(orderItem);
                SOFEntity.getDb().SaveChanges();

            }
            catch (Exception e)
            {



            }
            var basket = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();

            return PartialView("ItemBasket", basket);
        }

        public ActionResult ClearBasket()
        {
            int user = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var temp = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();
            if (temp.Any())
            {
                foreach (var item in temp)
                {
                    SOFEntity.getDb().OrderItems.Remove(item);


                }
                SOFEntity.getDb().SaveChanges();

            }


            var basket = SOFEntity.getDb().OrderItems.Where(o => o.Orders.UserID == user && o.Orders.OrderStatus == null).ToList();

            return PartialView("ItemBasket", basket);

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
            ViewBag.RestaurantID = SOFEntity.getDb().Orders.Where(o => o.UserID == userID && o.OrderStatus == null).FirstOrDefault().RestaurantID;

            return View();
        }

        [HttpPost]
        public ActionResult Payment([Bind(Include = "TotalPrice,Address,PaymentType,Description")] Orders orders)
        {


            int userID = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);


            var user = SOFEntity.getDb().Users.Find(userID);

            DateTime current = DateTime.Now;

            var order = SOFEntity.getDb().Orders.Where(o => o.UserID == userID && o.OrderStatus == null).FirstOrDefault();
            order.UserID = userID;
            order.Telephone = user.Telephone;
            order.Date = current;
            order.TotalPrice = orders.TotalPrice;
            order.Address = orders.Address;
            order.PaymentType = orders.PaymentType;
            order.Description = orders.Description;
            order.OrderStatus = 0;


            var newOrder = new SOF301.Models.Orders();
            newOrder.UserID = userID;


            try
            {
                SOFEntity.getDb().Orders.Add(newOrder);

                SOFEntity.getDb().SaveChanges();
            }
            catch (Exception e)
            {



            }
            return RedirectToAction("Index");

            //  ViewBag.UserID = new SelectList(SOFEntity.getDb().Users, "UserID", "UserName", orders.UserID);

        }



        public ActionResult ListOrders()
        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);


            var orders = SOFEntity.getDb().Orders.Where(o => o.UserID == userID && (o.OrderStatus == 0 || o.OrderStatus == 1 || o.OrderStatus == 2));
            return View(orders.ToList());
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //   var OrderID = SOFEntity.getDb().OrderItems.Find(id).OrderID;
            var list = SOFEntity.getDb().OrderItems.Where(o => o.OrderID == id).ToList();

            return View(list);
        }

    }
}