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
using Spoffice.Website.Models.Output;
using System.Globalization;

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
        public ActionResult Localization(string id)
        {
            return MultiformatView(typeof(LanguageOutput), new LanguageOutput(id));
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
