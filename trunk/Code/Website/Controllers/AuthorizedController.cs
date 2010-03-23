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
using Spoffice.Website.Services.Music;
using Spoffice.Website.Services.Music.Downloader;
using Spoffice.Website.Services.Music.Player;
using System.Configuration;

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
        public static MusicService MusicService;
        public static SpotifyDownloader Downloader;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (MusicService == null)
            {
                string spotifyUsername = ConfigurationSettings.AppSettings["Spotify.Username"];
                string spotifyPassword = ConfigurationSettings.AppSettings["Spotify.Password"];
                if (!String.IsNullOrEmpty(spotifyUsername) && !String.IsNullOrEmpty(spotifyPassword))
                {
                    if (Downloader == null)
                    {
                        Downloader = new SpotifyDownloader(spotifyUsername, spotifyPassword);
                    }
                    IrrklangMusicPlayer player = new IrrklangMusicPlayer(DataContext.TrackRepository.GetTotalBytesPlayed());
                    MusicService = new MusicService(Downloader, player);
                }
                else
                {
                    throw new Exception("No username or password specified in app settings!");
                }
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
