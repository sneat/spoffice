using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Output;
using System.Security.Principal;
using System.Web.Security;
using System.Xml.Serialization;
using System.IO;
using Spoffice.Website.Helpers;
using System.Xml.Linq;
using Spoffice.Website.Services.Music;
using Spoffice.Website.Models.Spotify;

namespace Spoffice.Website.Controllers
{
    [Authorize]
    public class MusicController : AuthorizedController
    {
        
        private CoverGrabber covergrabber;
        public MusicController()
        {
            List<ICoverGrabber> grabbers = new List<ICoverGrabber>();

            if (Downloader != null) grabbers.Add(Downloader);
            grabbers.Add(new LastFMCoverGrabber());

            covergrabber = new CoverGrabber(grabbers);
        }   
        //
        // GET: /Music/
        public ActionResult Index()
        {
            ModelState.Clear(); // Clear the model state so that we don't have the search term in the box anymore
            return View();
        }
        
        public ActionResult Search(string id)
        {
            ViewData["search_term"] = id;
            ModelState.Clear();
            return MultiformatView(typeof(TrackListOutput), new TrackListOutput { Tracks = MusicSearch.SearchForTrack(id) });
        }
        public ActionResult Current()
        {
            return View();
        }
        public ActionResult Skip(string id)
        {
            //Spoffice.Website.Helpers.SpotifyConnector.Current.Skip();
            return null;
        }

        public ActionResult Playlist()
        {
            int start = 0;
            if (!String.IsNullOrEmpty(Request.QueryString["from"]))
            {
                start = Convert.ToInt32(Request.QueryString["from"]);
            }
            int amount = 40;
            if (!String.IsNullOrEmpty(Request.QueryString["amount"]))
            {
                amount = Convert.ToInt32(Request.QueryString["amount"]);
            }
            return MultiformatView(typeof(TrackHistoryListOutput), new TrackHistoryListOutput
            {
                TrackHistories = DataContext.Context.TrackHistories.Include("Track.Artist").Include("Track.Album").OrderByDescending(t => t.Datetime).Skip(start).Take(amount).ToList()
            });
        }
        public ActionResult Artist(string id)
        {
            ModelState.Clear();
            return MultiformatView(typeof(ArtistOutput), MetadataApiParser.GetArtistById(id));
        }
        public ActionResult Album(string id)
        {
            ModelState.Clear();
            return MultiformatView(typeof(AlbumOutput), MetadataApiParser.GetAlbumById(id));
        }
        public RedirectResult TrackImage(string id)
        {
            return Redirect(covergrabber.GetCoverPath(new TrackOutput { PublicId = id }));
        }
        public RedirectResult AlbumImage(string id)
        {
            return Redirect(covergrabber.GetCoverPath(new AlbumOutput { PublicId = id }));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vote(string id, string value)
        {
            StatusOutput result = null;
            User user = (from m in DataContext.Context.Users
                         where m.UserId == UserGuid
                         select m).FirstOrDefault();

            switch (value)
            {
                case "for":
                    result = DataContext.RatingRepository.VoteForTrack(id, UserGuid);
                    break;
                case "against":
                    result = DataContext.RatingRepository.VoteAgainstTrack(id, UserGuid);
                    break;
            }

            return MultiformatView(typeof(StatusOutput), result);
        }


    }  
}
