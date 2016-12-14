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
    public class OrdersController : Controller
    {
        private SofModel db = new SofModel();

        // GET: Orders
        public ActionResult Index()
        {

         
           
            var userID =int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

          
            var orders = db.Orders.Where(o => o.UserID==userID);
            return View(orders.ToList());
        }

     


        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var OrderID = db.OrderItems.Find(id).OrderID;
            var list = db.OrderItems.Where(o => o.OrderID == OrderID).ToList();

            return View(list);
        }
    

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
