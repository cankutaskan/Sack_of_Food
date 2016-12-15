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
            ViewBag.CityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            return View();
        }

        [HttpPost]
        public ActionResult Register(Users model)
        {

            ViewBag.CityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");

            var user = SOFEntity.getDb().Users
                .Where(u => u.UserName == model.UserName)
                .Select(u => u).FirstOrDefault();  // if there is no user with given user name

            if (ModelState.IsValid && user == null) //Checks if input fields have the correct format
            {
                
                user = model;
                user.RoleID = 3;
                SOFEntity.getDb().Users.Add(user);
                SOFEntity.getDb().SaveChanges();

                return RedirectToAction("Login", "Auth");
            }
            else
            {
                ModelState.AddModelError("", "One or more fields have errors.");
            }
            return View(model);
        }

    }
}