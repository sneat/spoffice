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
using Spoffice.Website.Services.Music.Browser;

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
        public static IMusicBrowser Browser;
        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Browser == null)
            {
                Browser = new SpotifyBrowser();
            }
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
                    string mediaPlayer = ConfigurationSettings.AppSettings["Media.Player"];
                    if (!String.IsNullOrEmpty(mediaPlayer))
                    {
                        IMusicPlayer player;
                        if (mediaPlayer == "Irrklang")
                        {
                            player = new IrrklangMusicPlayer(DataContext.TrackRepository.GetTotalBytesPlayed());
                        }
                        else if (mediaPlayer == "WMP")
                        {
                            player = new WMPMusicPlayer(DataContext.TrackRepository.GetTotalBytesPlayed());
                        }
                        else
                        {
                            throw new Exception("Media player specified in app settings is not a valid option.");
                        }
                        MusicService = new MusicService(Downloader, player);
                    }
                    else
                    {
                        throw new Exception("No media player specified in app settings!");
                    }
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
