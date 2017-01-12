﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SOF301.Models;
using System.Security.Claims;
using System.Data.Entity;
using SOF301.Tools;
using System.Net;
using System.Data.Entity.Validation;
using System.Diagnostics;

namespace SOF301.Controllers
{
    [AllowAnonymous]
    public class AuthController : Controller
    {
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
            

            Users user = SOFEntity.getDb().Users.Where(u => u.UserName == model.UserName).FirstOrDefault();

            if (user != null)
            {
                var decryptedPassword = CustomDecrypt.Decrypt(user.Password);

                if (model.Password == decryptedPassword)
                {

                    var identity = new ClaimsIdentity(new[]
                    {
                    new Claim(ClaimTypes.Sid, user.UserID.ToString()), //user id cookie
                    new Claim(ClaimTypes.Role, user.RoleID.ToString()), //role id cookie
                    new Claim(ClaimTypes.Name, user.UserName)
                    }, "ApplicationCookie");

                    var temp = SOFEntity.getDb().Orders.Where(o => o.OrderStatus == null && o.UserID == user.UserID).ToList();
                     
                    if (temp.Any())
                    {
                        foreach (var item in temp)
                        {
                            var basket = SOFEntity.getDb().OrderItems.Where(o => o.OrderID == item.OrderID).ToList();
                            foreach(var oitem in basket)
                            {
                                SOFEntity.getDb().OrderItems.Remove(oitem);


                            }
                         
                            SOFEntity.getDb().Orders.Remove(item);
                            SOFEntity.getDb().SaveChanges();
                        }

                    }
                   
                    
                        var order = new SOF301.Models.Orders();

                        order.UserID = user.UserID;
                        try
                        {
                            SOFEntity.getDb().Orders.Add(order);
                            SOFEntity.getDb().SaveChanges();
                        }
                        catch (Exception e)
                        {
                            ViewData["EditError"] = e.Message;
                        }
                    

                    var ctx = Request.GetOwinContext();
                    var authManager = ctx.Authentication;
                    authManager.SignIn(identity);
                    switch (user.RoleID)
                    {
                        case 1:
                            return RedirectToAction("Index", "Admin");
                        case 2:
                            return RedirectToAction("Index", "RestaurantOwner");
                        case 3:
                            return RedirectToAction("Index", "Customer");
                        default:
                            return RedirectToAction("Index", "Home");
                    }



                }
                else
                {
                    ModelState.AddModelError("", "Incorrect Password!");
                    return View(model);
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user name!");
                return View(model);
            }
        }

        public ActionResult Logout()
        {
            var ctx = Request.GetOwinContext();
            var authManager = ctx.Authentication;
            int userId = int.Parse(ClaimsPrincipal.Current.FindFirst(ClaimTypes.Sid).Value);
            var order = SOFEntity.getDb().Orders.Where(o => o.UserID == userId);
            var temp = order.Where(o => o.OrderStatus == null).FirstOrDefault();
            
            var orderItems = SOFEntity.getDb().OrderItems.Where(o => o.Orders.OrderStatus == null && o.Orders.UserID == userId).ToList();

            try
            {
                foreach (var item in orderItems)
                {
                    SOFEntity.getDb().OrderItems.Remove(item);


                }
                SOFEntity.getDb().Orders.Remove(temp);
                SOFEntity.getDb().SaveChanges();

            }
            catch (Exception e)
            {

                ViewData["EditError"] = e.Message;
            }

            authManager.SignOut("ApplicationCookie");
            return RedirectToAction("Login", "Auth");
        }

        public ActionResult Register()
        {
            ViewBag.UserCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            ViewBag.RestaurantCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            ViewBag.UserDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");
            ViewBag.RestaurantDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");
            
            //try to use js callback to show City's Districts
            return View();
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

        [HttpPost]
        public ActionResult Register(RegisterViewModel model, bool isRestaurantOwner = false)
        {

            ViewBag.UserCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            ViewBag.RestaurantCityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name");
            ViewBag.UserDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");
            ViewBag.RestaurantDistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name");

            var user = SOFEntity.getDb().Users
                .Where(u => u.UserName == model.Users.UserName)
                .Select(u => u).FirstOrDefault();  // if there is no user with given user name

            if (ModelState.IsValid && user == null) //Checks if input fields have the correct format
            {
                if (string.IsNullOrWhiteSpace(model.Users.Email))
                {
                    ModelState.AddModelError("", "Email can not be blank!");
                    return View(model);
                }
                if (!isRestaurantOwner)
                {
                    user = model.Users;
                    var encryptedPassword = CustomEnrypt.Encrypt(user.Password);
                    user.Password = encryptedPassword;
                    user.RoleID = 3;                    //makes it customer
                    SOFEntity.getDb().Users.Add(user);
                    try
                    {
                        SOFEntity.getDb().SaveChanges();
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
                    

                    return RedirectToAction("Login", "Auth");

                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(model.Restaurants.Name))
                    {
                        user = model.Users;
                        var encryptedPassword = CustomEnrypt.Encrypt(user.Password);
                        user.Password = encryptedPassword;
                        user.RoleID = 2;                    //makes it owner

                        SOFEntity.getDb().Users.Add(user);
                        SOFEntity.getDb().SaveChanges();

                        var rest = SOFEntity.getDb().Restaurants
                            .Where(r => r.Name == model.Restaurants.Name)
                            .Select(r => r).FirstOrDefault();

                        if (rest == null)

                        {
                            rest = model.Restaurants;
                            rest.UserID = user.UserID;
                            rest.RestaurantStatu = false;

                            SOFEntity.getDb().Restaurants.Add(rest);
                            SOFEntity.getDb().SaveChanges();

                            Requests r = new Requests();
                            r.UserID = user.UserID;
                            r.RestaurantID = rest.RestaurantID;
                            SOFEntity.getDb().Requests.Add(r);
                            SOFEntity.getDb().SaveChanges();

                            return RedirectToAction("Login", "Auth");
                        }
                        else
                        {
                            ModelState.AddModelError("", "Restaurant name already exist.");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Restaurant name can't be blank.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("", "One or more fields have errors.");
            }
            return View(model);
        }
        
        public ActionResult Profile(int? id)

        {
            var userID = int.Parse(ClaimsPrincipal.Current.FindAll(ClaimTypes.Sid).ToList()[0].Value);

            Users users = SOFEntity.getDb().Users.Find(userID);

            return View(users);
        }
        
        public ActionResult EditUser(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Users users = SOFEntity.getDb().Users.Find(id);
            if (users == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name", users.CityID);
            ViewBag.RoleID = new SelectList(SOFEntity.getDb().Roles, "RoleID", "Name", users.RoleID);

            ViewBag.DistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name", users.DistrictID);

            //users.Password = CustomDecrypt.Decrypt(users.Password);

            return View(users);
        }
        
        [HttpPost]
        public ActionResult EditUser([Bind(Include = "UserID,RoleID,UserName,Name,Surname,Telephone,Address,CityID,DistrictID,Email")] Users users)
        {
            if(SOFEntity.getDb().Users.Where(u=>u.UserName == users.UserName && u.UserID != users.UserID).ToList().Any())
            {
                ModelState.AddModelError("", "User name already exist!");
            }
            else
            {
                if (ModelState.IsValid)
                {

                    Users originalUser = SOFEntity.getDb().Users.Find(users.UserID);
                    //users.Password = CustomEnrypt.Encrypt(users.Password);

                    SOFEntity.getDb().Entry(originalUser).CurrentValues.SetValues(users);
                    //SOFEntity.getDb().Entry(users).State = EntityState.Modified;

                    SOFEntity.getDb().SaveChanges();
                    return RedirectToAction("Profile");
                }
            }
            
            ViewBag.CityID = new SelectList(SOFEntity.getDb().Cities, "CityID", "Name", users.CityID);
            ViewBag.RoleID = new SelectList(SOFEntity.getDb().Roles, "RoleID", "Name", users.RoleID);
            ViewBag.DistrictID = new SelectList(SOFEntity.getDb().Districts, "DistrictID", "Name", users.DistrictID);
            return View(users);
        }

        public ActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ForgotPassword([Bind(Include = "UserID,UserName,Email")] Users u)
        {
            Users user = SOFEntity.getDb().Users.Where(t => t.UserName == u.UserName).First();
            if (user != null)
            {
                MailHandler.SendForgottenPassword(user);
            }
            else
            {
                ModelState.AddModelError("", "Invalid username!");
            }
            return View(u);
        }
        

    }
}