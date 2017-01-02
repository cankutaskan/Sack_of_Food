using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOF301.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("3"))
                {
                    return RedirectToAction("Index", "Customer");
                }
                if (User.IsInRole("2"))
                {
                    return RedirectToAction("Index", "RestaurantOwner");
                }
                if (User.IsInRole("1"))
                {
                    return RedirectToAction("Index", "Admin");
                }
            }
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}