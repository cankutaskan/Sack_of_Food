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
    public class UsersController : Controller
    {
        private SofModel db = new SofModel();

     

        // GET: Users/Details/5
        public ActionResult Details(int? id)
          
        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);
          
            Users users = db.Users.Find(userID);

            return View(users);
        }
        // GET: Users1/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = db.Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", users.CityID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", users.RoleID);
            return View(users);
        }

        // POST: Users1/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]

        public ActionResult Edit([Bind(Include = "UserID,RoleID,UserName,Password,Name,Surname,Telephone,Address,CityID,Email")] Users users)
        {
            if (ModelState.IsValid)
            {
                db.Entry(users).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Details");
            }
            ViewBag.CityID = new SelectList(db.Cities, "CityID", "Name", users.CityID);
            ViewBag.RoleID = new SelectList(db.Roles, "RoleID", "Name", users.RoleID);
            return View(users);
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
