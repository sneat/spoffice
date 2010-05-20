using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models
{
    public class RatingRepository : IRatingRepository
    {
 
        public RatingRepository()
        {
        }
        #region IRatingRepository Members

        public Rating GetTrackRatingForUser(Track track, User user)
        {
            return (from m in DataContext.Context.Ratings
                      where m.Track.Id == track.Id && m.User.UserId == user.UserId
                      select m).FirstOrDefault();
        }

        public void DeleteRatingsForTrack(Track track)
        {
            List<Rating> ratings = (from m in DataContext.Context.Ratings
                    where m.Track.Id == track.Id
                    select m).ToList();

            foreach (Rating rating in ratings)
            {
                DataContext.Context.DeleteObject(rating);
            }
            DataContext.Context.SaveChanges();
        }
        public StatusOutput VoteForTrack(Guid trackid, Guid userGuid)
        {
            Track track = DataContext.TrackRepository.GetTrackById(trackid);
            Rating rating = (from m in DataContext.Context.Ratings
                             where m.Track.Id == track.Id && m.User.UserId == userGuid
                             select m).FirstOrDefault();
            if (rating != null)
            {
                DataContext.Context.DeleteObject(rating);
                DataContext.Context.SaveChanges();
            }
            return DataContext.FavouriteRepository.AddToFavourites(trackid, userGuid);
        }
        public StatusOutput VoteForTrack(Track track, Guid userGuid)
        {
            return VoteForTrack(track.Id, userGuid);
        }
        public StatusOutput VoteAgainstTrack(Guid trackid, Guid userGuid)
        {
            return VoteAgainstTrack(DataContext.TrackRepository.GetTrackById(trackid), userGuid);
        }
        public StatusOutput VoteAgainstTrack(Track track, Guid userGuid)
        {
            if (track != null)
            {
                User user = (from m in DataContext.Context.Users
                             where m.UserId == userGuid
                             select m).FirstOrDefault();

                Rating rating = GetTrackRatingForUser(track, user);
                if (rating != null)
                {
                    return new StatusOutput { StatusCode = "Error", Message = Res.Strings.RatingsFailedVoteAgainst };
                }
                Favourite favourite = DataContext.FavouriteRepository.GetTrackFavouriteForUser(track.Id, user.UserId);
                if (favourite != null)
                {
                    DataContext.FavouriteRepository.RemoveFromFavourites(track.Id, user.UserId);
                }
                DataContext.Context.AddToRatings(new Rating
                {
                    Track = track,
                    User = user,
                    Value = -1
                });
                DataContext.Context.SaveChanges();
                return new StatusOutput { StatusCode = "Success", Message = Res.Strings.RatingsSuccessVoteAgainst };
            }
            return new StatusOutput { StatusCode = "Error", Message = Res.Strings.RatingsUnknownTracKVoteAgainst };
        }
        #endregion
    }
}
