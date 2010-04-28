using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;
using Spoffice.Website.Services;
using Spoffice.Website.Controllers;

namespace Spoffice.Website.Models
{
    public class FavouriteRepository : IFavouriteRepository
    {

        #region IFavouriteRepository Members

        public List<Favourite> GetUsersFavourites(Guid userid)
        {
            return DataContext.Context.Favourites.Include("Track.Artist").Include("Track.Album").Where(f => f.User.UserId == userid).ToList();
        }


        public Favourite GetTrackFavouriteForUser(Guid trackid, Guid userid)
        {
            return (from m in DataContext.Context.Favourites
                    where m.Track.Id == trackid && m.User.UserId == userid
                    select m).FirstOrDefault();
        }

        #endregion

        #region IFavouriteRepository Members


        public StatusOutput AddToFavourites(Guid trackid, Guid userid)
        {
            StatusOutput result = new StatusOutput();
            Boolean dbChanged = false;
            TrackOutput trackNode = AuthorizedController.Browser.GetTrackById(trackid);
            ArtistOutput artistNode = trackNode.Artist;
            AlbumOutput albumNode = trackNode.Album;

            Artist artist = DataContext.ArtistRepository.GetArtistById(artistNode.Id);

            if (artist == null)
            {
                artist = new Artist
                {
                    Name = artistNode.Name,
                    Id = artistNode.Id,
                    MusicBrainzId = trackNode.MusicBrainzId
                };
                DataContext.Context.AddToArtists(artist);
                dbChanged = true;
            }

            Album album = DataContext.AlbumRepository.GetAlbumById(albumNode.Id);

            if (album == null)
            {
                album = new Album
                {
                    Name = albumNode.Name,
                    Id = albumNode.Id,
                    MusicBrainzId = albumNode.MusicBrainzId
                };
                DataContext.Context.AddToAlbums(album);
                dbChanged = true;
            }

            Track track = DataContext.TrackRepository.GetTrackById(trackNode.Id);

            if (track == null)
            {
                track = new Track
                {
                    Artist = artist,
                    Album = album,
                    Title = trackNode.Title,
                    Id = trackNode.Id,
                    Length = trackNode.Length,
                    MusicBrainzId = trackNode.MusicBrainzId
                };
                DataContext.Context.AddToTracks(track);
                dbChanged = true;
            }

            User user = (from m in DataContext.Context.Users
                         where m.UserId == userid
                         select m).FirstOrDefault();

            Favourite favourite = GetTrackFavouriteForUser(track.Id, userid);

            if (favourite == null)
            {
                favourite = new Favourite
                {
                    User = user,
                    Track = track
                };
                DataContext.Context.AddToFavourites(favourite);
                dbChanged = true;
            }
            if (dbChanged)
            {
                try
                {
                    DataContext.Context.SaveChanges();
                    result = new StatusOutput
                    {
                        StatusCode = "Success",
                        Favourite = favourite.AsOutput(),
                        Message = Res.Strings.FavouritesSuccessAdd
                    };
                }
                catch
                {
                    result = new StatusOutput
                    {
                        StatusCode = "Error",
                        Message = Res.Strings.FavouritesFailedAdd
                    };
                }
            }
            else
            {
                result = new StatusOutput
                {
                    StatusCode = "Error",
                    Message = Res.Strings.FavouritesFailedNothingAdd
                };
            }
            return result;
        }

        public StatusOutput RemoveFromFavourites(Guid privateId, Guid userid)
        {
            StatusOutput result;


            User user = (from m in DataContext.Context.Users
                         where m.UserId == userid
                         select m).FirstOrDefault();

            Favourite favourite = GetTrackFavouriteForUser(privateId, userid);
          
            if (favourite != null)
            {
                try
                {
                    DataContext.Context.DeleteObject(favourite);

                    DataContext.Context.SaveChanges();

                    result = new StatusOutput
                    {
                        StatusCode = "Success",
                        Message = Res.Strings.FavouritesSuccessRemove
                    };
                }
                catch
                {
                    result = new StatusOutput
                    {
                        StatusCode = "Error",
                        Message = Res.Strings.FavouritesFailedRemove
                    };
                }
            }
            else
            {
                result = new StatusOutput
                {
                    StatusCode = "Error",
                    Message = Res.Strings.FavouritesFailedRemoveNotFound
                };
            }

            return result;
        }

        #endregion
    }
}
