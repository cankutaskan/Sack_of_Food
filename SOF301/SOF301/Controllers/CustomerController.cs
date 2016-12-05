using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;

namespace SOF301.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Customer

        public ActionResult Index(String sortOrder, string currentFilter, string searchString, int? page)
        {
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

            SOF301.Models.SofModel db = new Models.SofModel();
            var restaurant = db.Restaurants.Select(r => r);
            if (!String.IsNullOrEmpty(searchString))
            {
                restaurant = restaurant.Where(s => s.Name.Contains(searchString));
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
    }
}