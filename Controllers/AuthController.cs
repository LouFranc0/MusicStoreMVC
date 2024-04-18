using EcommerceChitarre.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace EcommerceChitarre.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth
        DBContext db = new DBContext();
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(Users user)
        {
            var loggedUser = db.Users.Where(u => u.Email == user.Email && u.Password == user.Password).FirstOrDefault();

            if (loggedUser == null)
            {
                TempData["ErrorLogin"] = true;
                return RedirectToAction("Login");
            }

            FormsAuthentication.SetAuthCookie(loggedUser.User_ID.ToString(), true);
            return RedirectToAction("Index", "Home");
        }



        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login"); 
        }

    }
}