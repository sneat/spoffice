using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web.Security;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Spoffice.Website.Helpers;
using Spoffice.Website.Models;
using Spoffice.Website.Services;
using System.Configuration;
using Microsoft.Practices.Unity;

namespace Spoffice.Website.Controllers
{
    public class NotAuthorizeAttribute : FilterAttribute
    {
        // Does nothing, just used for decoration
    }
    public class AuthorizedController : BaseController
    {
        protected Guid UserGuid
        {
            get
            {
                return (Guid)Membership.GetUser().ProviderUserKey;
            }
        }
        protected IUnityContainer myContainer = null;
        public static IMusicBrowser Browser;
        public static MusicService MusicService;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Object retrievedContainer = HttpContext.Application["ServicesContainer"];
            myContainer = retrievedContainer as IUnityContainer;
            if (Browser == null)
            {                
                Browser = myContainer.Resolve<IMusicBrowser>();
                if (Browser == null)
                {
                    throw new Exception(Res.Strings.MusicBrowserNotSpecified);
                }
            }
            if (MusicService == null)
            {
                
                IMusicDownloader musicDownloader = myContainer.Resolve<IMusicDownloader>();
                if (musicDownloader == null)
                {
                    throw new Exception(Res.Strings.MusicDownloaderNotSpecified);
                }
                IMusicPlayer musicPlayer = myContainer.Resolve<IMusicPlayer>();
                if (musicPlayer == null)
                {
                    throw new Exception(Res.Strings.MusicPlayerNotSpecified);
                }
                MusicService = new MusicService(musicPlayer, musicDownloader);
            }

            // Check if this action has NotAuthorizeAttribute
            object[] attributes = filterContext.ActionDescriptor.GetCustomAttributes(true);
            if (attributes.Any(a => a is NotAuthorizeAttribute)) return;

            // Must login
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
        }
    }
}
