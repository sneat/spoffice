using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Spotify.MetadataApi;
using System.Security.Principal;
using System.Web.Security;
using System.Xml.Serialization;
using System.IO;
using Spoffice.Website.Helpers;
using System.Xml.Linq;
using Spoffice.Website.Services.Music;

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

            ViewData["favourites"] = DataContext.FavouriteRepository.GetUsersFavourites(UserGuid);
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
            return MultiformatView(typeof(TrackList), new TrackList { Tracks = MusicSearch.SearchForTrack(id) });
        }
        public ActionResult Current()
        {
            return MultiformatView(typeof(TrackProgress), new TrackProgress());
        }
        public ActionResult Skip(string id)
        {
            //Spoffice.Website.Helpers.SpotifyConnector.Current.Skip();
            return null;
        }
        public ActionResult Playlist()
        {
            return MultiformatView(typeof(TrackHistoryList), new TrackHistoryList
            {
                TrackHistories = DataContext.Context.TrackHistories.Include("Track.Artist").Include("Track.Album").Top("10").OrderByDescending(t => t.Datetime).ToList()
            });
        }
        public ActionResult Artist(string id)
        {
            ModelState.Clear();
            return MultiformatView(typeof(ArtistNode), new ArtistNode(id));
        }
        public ActionResult Album(string id)
        {
            ModelState.Clear();
            return MultiformatView(typeof(AlbumNode), new AlbumNode(id));
        }
        public RedirectResult TrackImage(string id)
        {
            return Redirect(covergrabber.GetCoverPath(new TrackNode(id)));
        }
        public RedirectResult AlbumImage(string id)
        {
            return Redirect(covergrabber.GetCoverPath(new AlbumNode(id)));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vote(string id, string value)
        {
            Status result = null;
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

            return MultiformatView(typeof(Status), result);
        }


    }  
}
