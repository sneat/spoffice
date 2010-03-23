using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;
using System.Web.Security;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Spotify.MetadataApi;

namespace Spoffice.Website.Controllers
{
    [Authorize]
    public class FavouritesController : BaseController
    {
        spofficeEntities _db;
        Guid userGuid;

        public FavouritesController()
        {
            _db = new spofficeEntities();
            if (Membership.GetUser() != null)
            {
                userGuid = (Guid)Membership.GetUser().ProviderUserKey;
            }
        }

        //
        // GET: /Favourites/
        public ActionResult Index()
        {
          
            List<Favourite> favourites = _db.Favourites.Include("Track.Artist").Where(f => f.User.UserId == userGuid).ToList();
            return View(favourites);
        }

        //
        // GET: /Favourites/Add

        public ActionResult Add(string id)
        {
            JsonFavouriteResult result = new JsonFavouriteResult();
            Boolean dbChanged = false;

            TrackNode trackNode = new TrackNode(id);
            ArtistNode artistNode = trackNode.Artist;
            AlbumNode albumNode = trackNode.Album;

            Artist artist = (from m in _db.Artists
                             where m.Id == artistNode.PrivateId
                             select m).FirstOrDefault();


            if (artist == null)
            {
                artist = new Artist
                {
                    Name = artistNode.Name,
                    Id = artistNode.PrivateId,
                    MusicBrainzId = trackNode.MusicBrainzId
                };
                _db.AddToArtists(artist);
                dbChanged = true;
            }
            else if ((artist.MusicBrainzId == null || artist.MusicBrainzId == Guid.Empty) && artistNode.MusicBrainzId != null)
            {
                artist.MusicBrainzId = artistNode.MusicBrainzId;
                dbChanged = true;
            }

            Album album = (from m in _db.Albums
                           where m.Id == albumNode.PrivateId
                           select m).FirstOrDefault();
            if (album == null)
            {
                album = new Album
                {
                    Name = albumNode.Name,
                    Id = albumNode.PrivateId,
                    MusicBrainzId = albumNode.MusicBrainzId
                };
                _db.AddToAlbums(album);
                dbChanged = true;
            }
            else if ((album.MusicBrainzId == null || album.MusicBrainzId == Guid.Empty) && albumNode.MusicBrainzId != null)
            {
                album.MusicBrainzId = albumNode.MusicBrainzId;
                dbChanged = true;
            }

            Track track = (from m in _db.Tracks
                           where m.Id == trackNode.PrivateId
                           select m).FirstOrDefault();
            if (track == null)
            {
                track = new Track
                {
                    Artist = artist,
                    Album = album,
                    Title = trackNode.Title,
                    Id = trackNode.PrivateId,
                    Length = trackNode.Length,
                    MusicBrainzId = trackNode.MusicBrainzId
                };
                _db.AddToTracks(track);
                dbChanged = true;
            }
            else if ((track.MusicBrainzId == null || track.MusicBrainzId == Guid.Empty) && trackNode.MusicBrainzId != null)
            {
                track.MusicBrainzId = trackNode.MusicBrainzId;
                dbChanged = true;
            }

            User user = (from m in _db.Users
                         where m.UserId == userGuid
                         select m).FirstOrDefault();
            Favourite favourite = (from m in _db.Favourites
                                   where m.Track.Id == track.Id && m.User.UserId == user.UserId
                                   select m).FirstOrDefault();
            if (favourite == null)
            {
                favourite = new Favourite
                {
                    User = user,
                    Track = track
                };
                _db.AddToFavourites(favourite);
                dbChanged = true;
            }

            Rating rating = (from m in _db.Ratings
                             where m.Track.Id == track.Id && m.User.UserId == user.UserId
                             select m).FirstOrDefault();
            if (rating == null)
            {
                rating = new Rating
                {
                    Track = track,
                    User = user,
                    Value = 1
                };
                _db.AddToRatings(rating);
                dbChanged = true;
            }

            if (dbChanged)
            {
                try
                {
                    _db.SaveChanges();
                    result = new JsonFavouriteResult
                    {
                        Status = "OK"
                    };
                }
                catch
                {
                    result = new JsonFavouriteResult
                    {
                        Status = "Error",
                        Message = ViewRes.FavouritesStrings.ErrorSaving
                    };
                }
            }
            else
            {
                result = new JsonFavouriteResult
                {
                    Status = "OK",
                    Message = ViewRes.FavouritesStrings.NothingToAdd
                };
            }

            return Json(result);
        }

        //
        // DELETE: /Favourites/Remove
        public ActionResult Remove(string id)
        {
            ModelState.Clear(); // Clear the model state so that we don't have the search term in the box anymore
            Guid privateId = TrackNode.ConvertPublicToPrivate(id);
            User user = (from m in _db.Users
                         where m.UserId == userGuid
                         select m).FirstOrDefault();

            Favourite favourite = (from m in _db.Favourites.Include("Track")
                                   where m.Track.Id == privateId && m.User.UserId == user.UserId
                                   select m).FirstOrDefault();
            if (favourite == null)
                return View("NotFound");
            else
                return View(favourite);
        }

        [AcceptVerbs(HttpVerbs.Post), ValidateAntiForgeryToken]
        public ActionResult Remove(string id, bool isJs)
        {
            JsonFavouriteResult result;

            Guid privateId = TrackNode.ConvertPublicToPrivate(id);
            User user = (from m in _db.Users
                         where m.UserId == userGuid
                         select m).FirstOrDefault();

            Favourite favourite = (from m in _db.Favourites
                                   where m.Track.Id == privateId && m.User.UserId == user.UserId
                                   select m).FirstOrDefault();

            if (favourite != null)
            {
                try
                {
                    Rating rating = (from m in _db.Ratings
                                     where m.Track.Id == favourite.Track.Id && m.User.UserId == user.UserId
                                     select m).FirstOrDefault();
                    if (rating != null)
                    {
                        _db.DeleteObject(rating);
                    }
                    _db.DeleteObject(favourite);

                    _db.SaveChanges();

                    result = new JsonFavouriteResult
                    {
                        Status = "OK"
                    };
                }
                catch
                {
                    result = new JsonFavouriteResult
                    {
                        Status = "Error",
                        Message = ViewRes.FavouritesStrings.ErrorRemoving
                    };
                }
            }
            else
            {
                result = new JsonFavouriteResult
                {
                    Status = "Error",
                    Message = ViewRes.FavouritesStrings.FavouriteNotFound
                };
            }

            if (isJs)
            {
                return Json(result);
            }
            else
            {
                return RedirectToAction("Index");
            }
        } 
       
    }
    public class JsonFavouriteResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
    }
}
