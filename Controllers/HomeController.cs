using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace  EcommerceChitarre.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            if (User.IsInRole("Amministratore"))
            {
                return RedirectToAction("Index", "Ordini");
            }
                return View();
        }
    }
}
