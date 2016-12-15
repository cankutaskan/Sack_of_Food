using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOF301.Models;
using System.Security.Claims;
using System.Data.Entity;

namespace SOF301.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
        // GET: Auth
        [HttpGet]
        public ActionResult Login(string url)
        {
            
            return View();
        }
        
        [HttpPost]
        public ActionResult Login(Users model)
        {
            if (!ModelState.IsValid) //Checks if input fields have the correct format
            {

                Response.Write("model not valid");
                return View(model); //Returns the view with the input values so that the user doesn't have to retype again
            }

            int userId = new SofModel().Users
                .Where(u => u.UserName == model.UserName && u.Password == model.Password)
                .Select(u => u.UserID).FirstOrDefault();

            if (userId != 0)
            {
                var identity = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Sid, userId.ToString()), //user id cookie
                    new Claim(ClaimTypes.Role, new SofModel().Users.Where(u=>u.UserID == userId).Select(u=>u.RoleID).FirstOrDefault().ToString()) //role id cookie
                }, "ApplicationCookie");

                var ctx = Request.GetOwinContext();
                var authManager = ctx.Authentication;
                authManager.SignIn(identity);
                
                
                

                return RedirectToAction("Index","Home");
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password!");
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult Register()
        {
            ViewBag.UserCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            ViewBag.RestaurantCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            //ViewBag.UserDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");
            ViewBag.RestaurantDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");
            //js callback
            return View();
        }

        [HttpPost]
        public ActionResult Register(UserRestaurantViewModel model, bool isRestaurantOwner = false)
        {

            ViewBag.UserCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            ViewBag.RestaurantCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            //ViewBag.UserDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");
            ViewBag.RestaurantDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");

            var user = SOFEntity.getDb().Users
                .Where(u => u.UserName == model.Users.UserName)
                .Select(u => u).FirstOrDefault();  // if there is no user with given user name

            if (ModelState.IsValid && user == null) //Checks if input fields have the correct format
            {
                if (!isRestaurantOwner)
                {
                    user = model.Users;
                    user.RoleID = 3;                    //makes it customer
                    SOFEntity.getDb().Users.Add(user);
                    SOFEntity.getDb().SaveChanges();

                    return RedirectToAction("Login", "Auth");

                }
                else
                {
                    user = model.Users;
                    user.RoleID = 2;                    //makes it owner

                    SOFEntity.getDb().Users.Add(user);
                    SOFEntity.getDb().SaveChanges();

                    var rest = SOFEntity.getDb().Restaurants
                        .Where(r => r.Name == model.Restaurants.Name)
                        .Select(r => r).FirstOrDefault();
                    if(rest == null)
                    {
                        rest = model.Restaurants;
                        rest.UserID = user.UserID;
                        rest.RestaurantStatu = false;

                        SOFEntity.getDb().Restaurants.Add(rest);
                        SOFEntity.getDb().SaveChanges();

                        return RedirectToAction("Login", "Auth");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Restaurant name already exist.");
                    }
                }
                
            }
            else
            {
                ModelState.AddModelError("", "One or more fields have errors.");
            }
            return View(model);
        }

    }
}