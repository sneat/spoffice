using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Spoffice.Website.Controllers
{
    public class BaseController : Controller
    {
        public ActionResult MultiformatView(Type type, object obj)
        {
            return MultiformatView(type, obj, null);
        }
        public ActionResult MultiformatView(Type type, object obj, string redirect)
        {
            bool xml = IsXmlRequest();
            bool json = IsJsonRequest();
            if (xml || json)
            {
                string response;
                if (xml)
                {
                    StringWriter sw = new StringWriter();
                    new XmlSerializer(type).Serialize(sw, obj);
                    response = sw.ToString();
                    sw.Close();
                }
                else
                {
                    response = JsonConvert.SerializeObject(obj);
                }
                return new ContentResult
                {
                    ContentType = xml ? "text/xml" : "application/json",
                    Content = response
                };
            }
            else if (!String.IsNullOrEmpty(redirect))
            {
                return Redirect(redirect);
            }
            return View(obj);
        }
        protected bool IsJsonRequest()
        {
            return (Request.Headers["HTTP_X_EXPECT_FORMAT"] ?? string.Empty) == "json";
        }
        protected bool IsXmlRequest()
        {
            return (Request.Headers["HTTP_X_EXPECT_FORMAT"] ?? string.Empty) == "xml";
        }
    }
}
