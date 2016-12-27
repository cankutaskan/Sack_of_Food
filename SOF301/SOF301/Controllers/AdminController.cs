using SOF301.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOF301.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ListRequests()
        {
            var list = SOFEntity.getDb().Requests.Select(s => s).ToList();
            return View(list);
        }
        
        public ActionResult AcceptRequest(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Requests req = SOFEntity.getDb().Requests.Find(int.Parse(id));
                Restaurants res = SOFEntity.getDb().Restaurants.Find(req.RestaurantID);

                Restaurants updatedRes = res;
                updatedRes.RestaurantStatu = true;

                SOFEntity.getDb().Entry(res).CurrentValues.SetValues(updatedRes);
                SOFEntity.getDb().SaveChanges();

                if ((bool)res.RestaurantStatu)
                {
                    SOFEntity.getDb().Requests.Remove(req);
                        
                    SOFEntity.getDb().SaveChanges();                    
                }
            }

            return RedirectToAction("ListRequests");
        }

        public ActionResult RejectRequest(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                Requests req = SOFEntity.getDb().Requests.Find(int.Parse(id));
                Restaurants res = SOFEntity.getDb().Restaurants.Find(req.RestaurantID);
                if (!(bool)res.RestaurantStatu)
                {
                    SOFEntity.getDb().Restaurants.Remove(res);
                    SOFEntity.getDb().Requests.Remove(req);

                    SOFEntity.getDb().SaveChanges();
                }
            }

            return RedirectToAction("ListRequests");
        }

    }
}