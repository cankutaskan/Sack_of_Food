using SOF301.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SOF301.Controllers
{
    [Authorize(Roles = "1")]
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {
            return RedirectToAction("ListRequests");
        }

        public ActionResult ListRequests()
        {
            var list = SOFEntity.getDb().Requests.Select(s => s).ToList();
            return View(list);
        }

        public ActionResult ListUsers()
        {
            var list = SOFEntity.getDb().Users.Select(s => s).ToList();
            return View(list);
        }

        public ActionResult ListRestaurants()
        {
            var list = SOFEntity.getDb().Restaurants.Select(s => s).ToList();
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
        
        public ActionResult DeleteUser(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                int temp = int.Parse(id);
                Users u = SOFEntity.getDb().Users.Find(temp);
                if (u != null)
                {
                    SOFEntity.getDb().Users.Remove(u);
                    try
                    {
                        //SOFEntity.getDb().SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                Trace.TraceInformation("Property: {0} Error: {1}",
                                                        validationError.PropertyName,
                                                        validationError.ErrorMessage);
                            }
                        }
                    }
                }
            }

            return RedirectToAction("ListUsers");
        }

        public ActionResult DeleteRestaurant(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                int temp = int.Parse(id);
                Restaurants u = SOFEntity.getDb().Restaurants.Find(temp);
                if (u != null)
                {
                    SOFEntity.getDb().Restaurants.Remove(u);
                    //SOFEntity.getDb().SaveChanges();
                }
            }
            return RedirectToAction("ListRestaurants");
        }


    }
}