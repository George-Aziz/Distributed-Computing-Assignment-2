// Filename: HomeController.cs
// Project:  DC Assignment 2 (Practical 4)
// Purpose:  View Home Controller
// Author:   George Aziz (19765453)
// Date:     25/04/2021

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hybrid_Biz_Presentation_Tier.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "Bank of Aziz";
            return View();
        }

        public ActionResult Accounts()
        {
            ViewBag.Message = "Accounts Page";

            return View();
        }
        public ActionResult Users()
        {
            ViewBag.Message = "Users Page";

            return View();
        }
        public ActionResult Transactions()
        {
            ViewBag.Message = "Transactions Page";

            return View();
        }
    }
}