using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Security;
using System.Web;
using System.Web.Mvc;
using Spoffice.Website.Models;
using SharpSpotLib.Media;
using Spoffice.Website.Helpers;

namespace Spoffice.Website.Controllers
{
    [HandleError, Authorize]
    public class HomeController : AuthorizedController
    {
        spofficeEntities _db;

        public HomeController()
        {
            _db = new spofficeEntities();
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to Cooltunes!";
            return View();
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
