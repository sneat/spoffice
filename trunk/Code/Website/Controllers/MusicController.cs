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
using Microsoft.Practices.Unity;
using Spoffice.Website.Services;

namespace Spoffice.Website.Controllers
{
    [Authorize]
    public class MusicController : AuthorizedController
    {
        
        public MusicController()
        {
            /*List<ICoverGrabber> grabbers = new List<ICoverGrabber>();

            if (Downloader != null) grabbers.Add(Downloader);
            grabbers.Add(new LastFMCoverGrabber());

            covergrabber = new CoverGrabber(grabbers);*/
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
            return MultiformatView(typeof(TrackListOutput), new TrackListOutput { Tracks = Browser.SearchForTrack(id) });
        }
        public ActionResult Current()
        {
            return MultiformatView(typeof(PlayerStatusOutput), new PlayerStatusOutput
            {
                PlayerPosition = MusicService.CurrentTrack != null ? MusicService.CurrentTrack.Progress : 0,
                TotalBytes = MusicService.TotalBytes,
                Tracks  = MusicService.UpcomingTracks.Select(t=> t.AsOutput()).ToList(),
                NumberOfVotes = MusicService.VoteCount,
                NumberOfVotesRequired = MusicService.RequiredVotes
            });
        }
        public ActionResult Skip()
        {
            MusicService.CurrentTrack.Stop();
            return null;
        }

        public ActionResult Playlist()
        {
            int start = String.IsNullOrEmpty(Request.QueryString["from"]) ? 0 : Convert.ToInt32(Request.QueryString["from"]);
            int amount = String.IsNullOrEmpty(Request.QueryString["amount"]) ? 40 : Convert.ToInt32(Request.QueryString["amount"]);

            return MultiformatView(typeof(TrackHistoryListOutput), new TrackHistoryListOutput(DataContext.Context.TrackHistories.Include("Track.Artist").Include("Track.Album").OrderByDescending(t => t.Datetime).Skip(start).Take(amount).ToList()));
        }
        public ActionResult Artist(string id)
        {
            return MultiformatView(typeof(ArtistOutput), Browser.GetArtistById(new Guid(id)));
        }
        public ActionResult Album(string id)
        {
            return MultiformatView(typeof(AlbumOutput), Browser.GetAlbumById(new Guid(id)));
        }
        public RedirectResult AlbumImage(string id)
        {
            string cover = null;
            AlbumOutput album = new AlbumOutput { Id = new Guid(id) };
            foreach (ICoverGrabber grabber in myContainer.ResolveAll<ICoverGrabber>())
            {
                cover = grabber.GetCoverPath(album);
                if (!String.IsNullOrEmpty(cover))
                {
                    break;
                }
            }
            if (String.IsNullOrEmpty(cover))
            {
                cover = "~/Content/blank_album.png";
            }
            return Redirect(cover);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Vote(string id, string value)
        {
            StatusOutput result = null;
            
            switch (value)
            {
                case "for":
                    result = DataContext.RatingRepository.VoteForTrack(new Guid(id), UserGuid);
                    break;
                case "against":
                    result = DataContext.RatingRepository.VoteAgainstTrack(new Guid(id), UserGuid);
                    if (MusicService.CurrentTrack != null && MusicService.CurrentTrack.Id == new Guid(id))
                    {
                        // Add a vote to skip
                        MusicService.AddVote(UserGuid);
                    }
                    break;
            }

            return MultiformatView(typeof(StatusOutput), result);
        }


    }  
}
