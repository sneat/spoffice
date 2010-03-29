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
    [HandleError]
    public class HomeController : BaseController
    {
        public HomeController()
        {
        }

        public ActionResult Index()
        {
            ViewData["Message"] = "Welcome to Cooltunes!";
            return View();
        }
        public ActionResult Localization()
        {
            System.Resources.ResourceReader reader = new System.Resources.ResourceReader(Server.MapPath("~/Resources/Views/Account/AccountStrings.resx"));
            return MultiformatView(typeof(System.Resources.ResourceReader), reader);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
