using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

namespace Spoffice.Website.Controllers
{
    public class AjaxHtmlController : Controller
    {
        //
        // GET: /AjaxHtml/

        public ActionResult LoginForm()
        {
            return PartialView("LoginForm");
        }
        public ActionResult Welcome()
        {
            return PartialView("Welcome");
        }
        public ActionResult Tabs()
        {
            return PartialView("Tabs");
        }

    }
}
