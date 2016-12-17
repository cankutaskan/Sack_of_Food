using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

using System.Security.Claims;

namespace SOF301.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer
        SOF301.Models.SofModel db = new Models.SofModel();
        

        public ActionResult Index(String sortOrder, string currentFilter, string searchString, int? page)
        {
          
            int x = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var districtID = db.Users.Where(u => u.UserID == x).Select(r=>r.DistrictID).FirstOrDefault();
        
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


            var restaurant = db.Restaurants.Where(r=> r.DistrictID== districtID);
            if (!String.IsNullOrEmpty(searchString))
            {
          
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
        [HttpPost]
        public ActionResult RestaurantPage(int? RestaurantID)
        {
            var foods = db.Foods.Where(r => r.RestaurantID == RestaurantID);
            ViewBag.Restaurant = RestaurantID;
            return View(foods.ToList());

        }
        [HttpGet]
        public ActionResult RestaurantPage()
        {

            int RestaurantID = ViewBag.Restaurant;



            var foods = db.Foods.Where(r => r.RestaurantID == RestaurantID);
           
            return View(foods.ToList());

        }



        public ActionResult ItemBasket(int? FoodID)
        {

            var order = new SOF301.Models.OrderItems();
            
            var RestaurantID = ViewBag.Restaurant;
            order.FoodID = FoodID;
            try
            {


                db.OrderItems.Add(order);
                db.SaveChanges();
            }
            catch
            {
                                
            }
            return RedirectToAction("RestaurantPage");
        }
    } }