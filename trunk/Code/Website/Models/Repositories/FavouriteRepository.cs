using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;
using Spoffice.Website.Models.Spotify;

namespace Spoffice.Website.Models
{
    public class FavouriteRepository : IFavouriteRepository
    {

        #region IFavouriteRepository Members

        public List<Favourite> GetUsersFavourites(Guid userid)
        {
            return DataContext.Context.Favourites.Include("Track.Artist").Where(f => f.User.UserId == userid).ToList();
        }

        #endregion

        #region IFavouriteRepository Members


        public Favourite GetTrackFavouriteForUser(Guid trackid, Guid userid)
        {
            return (from m in DataContext.Context.Favourites
                    where m.Track.Id == trackid && m.User.UserId == userid
                    select m).FirstOrDefault();
        }

        #endregion

        #region IFavouriteRepository Members


        public StatusOutput AddToFavourites(string trackid, Guid userid)
        {
            StatusOutput result = new StatusOutput();
            Boolean dbChanged = false;

            TrackOutput trackNode = MetadataApiParser.GetTrackById(trackid);
            ArtistOutput artistNode = trackNode.Artist;
            AlbumOutput albumNode = trackNode.Album;

            Artist artist = DataContext.ArtistRepository.GetArtistById(artistNode.PrivateId);

            if (artist == null)
            {
                artist = new Artist
                {
                    Name = artistNode.Name,
                    Id = artistNode.PrivateId,
                    MusicBrainzId = trackNode.MusicBrainzId
                };
                DataContext.Context.AddToArtists(artist);
                dbChanged = true;
            }
            else if ((artist.MusicBrainzId == null || artist.MusicBrainzId == Guid.Empty) && artistNode.MusicBrainzId != null)
            {
                artist.MusicBrainzId = artistNode.MusicBrainzId;
                dbChanged = true;
            }

            Album album = DataContext.AlbumRepository.GetAlbumById(albumNode.PrivateId);

            if (album == null)
            {
                album = new Album
                {
                    Name = albumNode.Name,
                    Id = albumNode.PrivateId,
                    MusicBrainzId = albumNode.MusicBrainzId
                };
                DataContext.Context.AddToAlbums(album);
                dbChanged = true;
            }
            else if ((album.MusicBrainzId == null || album.MusicBrainzId == Guid.Empty) && albumNode.MusicBrainzId != null)
            {
                album.MusicBrainzId = albumNode.MusicBrainzId;
                dbChanged = true;
            }

            Track track = DataContext.TrackRepository.GetTrackById(trackNode.PrivateId);

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
                DataContext.Context.AddToTracks(track);
                dbChanged = true;
            }
            else if ((track.MusicBrainzId == null || track.MusicBrainzId == Guid.Empty) && trackNode.MusicBrainzId != null)
            {
                track.MusicBrainzId = trackNode.MusicBrainzId;
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
            DataContext.RatingRepository.VoteForTrack(trackNode.PublicId, user.UserId);
            if (dbChanged)
            {
                try
                {
                    DataContext.Context.SaveChanges();
                    result = new StatusOutput
                    {
                        StatusCode = "OK"
                    };
                }
                catch
                {
                    result = new StatusOutput
                    {
                        StatusCode = "Error",
                        Message = ViewRes.FavouritesStrings.ErrorSaving
                    };
                }
            }
            else
            {
                result = new StatusOutput
                {
                    StatusCode = "OK",
                    Message = ViewRes.FavouritesStrings.NothingToAdd
                };
            }
            return result;
        }

        public StatusOutput RemoveFromFavourites(string trackid, Guid userid)
        {
            StatusOutput result;

            Guid privateId = TrackOutput.ConvertPublicToPrivate(trackid);

            User user = (from m in DataContext.Context.Users
                         where m.UserId == userid
                         select m).FirstOrDefault();

            Favourite favourite = GetTrackFavouriteForUser(privateId, userid);
          
            if (favourite != null)
            {
                try
                {
                    Rating rating = DataContext.RatingRepository.GetTrackRatingForUser(new Track{ Id = privateId}, user);
                    if (rating != null)
                    {
                        DataContext.Context.DeleteObject(rating);
                    }
                    DataContext.Context.DeleteObject(favourite);

                    DataContext.Context.SaveChanges();

                    result = new StatusOutput
                    {
                        StatusCode = "OK"
                    };
                }
                catch
                {
                    result = new StatusOutput
                    {
                        StatusCode = "Error",
                        Message = ViewRes.FavouritesStrings.ErrorRemoving
                    };
                }
            }
            else
            {
                result = new StatusOutput
                {
                    StatusCode = "Error",
                    Message = ViewRes.FavouritesStrings.FavouriteNotFound
                };
            }

            return result;
        }

        #endregion
    }
}
