using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using SharpSpotLib.Media;
using Spoffice.Website.Helpers;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Output;

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
