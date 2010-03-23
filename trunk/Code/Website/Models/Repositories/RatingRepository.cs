using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
        public Status VoteForTrack(string trackid, Guid userGuid)
        {
            return VoteForTrack(DataContext.TrackRepository.GetTrackById(trackid), userGuid);
        }
        public Status VoteForTrack(Track track, Guid userGuid)
        {
            if (track != null)
            {
                User user = (from m in DataContext.Context.Users
                             where m.UserId == userGuid
                             select m).FirstOrDefault();

                Rating rating = DataContext.RatingRepository.GetTrackRatingForUser(track, user);

                if (rating != null)
                {
                    if (rating.Value == -1)
                    {
                        DataContext.Context.DeleteObject(rating);
                        DataContext.Context.SaveChanges();
                    }
                    else
                    {
                        return new Status { StatusCode = "Error", Message = "You've already voted for this"};
                    }
                }
                DataContext.Context.AddToRatings(new Rating
                {
                    Track = track,
                    User = user,
                    Value = 1
                });
                DataContext.Context.SaveChanges();

                return new Status { StatusCode = "Success", Message = "Track successfully voted for" };
            }

            return new Status { StatusCode = "Error", Message = "Track unknown" };
        }
        public Status VoteAgainstTrack(string trackid, Guid userGuid)
        {
            return VoteAgainstTrack(DataContext.TrackRepository.GetTrackById(trackid), userGuid);
        }
        public Status VoteAgainstTrack(Track track, Guid userGuid)
        {
            if (track != null)
            {
                User user = (from m in DataContext.Context.Users
                             where m.UserId == userGuid
                             select m).FirstOrDefault();

                Rating rating = GetTrackRatingForUser(track, user);
                if (rating != null)
                {
                    if (rating.Value == 1)
                    {
                        // Had previously voted for
                        DataContext.Context.DeleteObject(rating);
                        DataContext.Context.SaveChanges();
                    }
                    else
                    {
                        return new Status { StatusCode = "Error", Message = "You've already voted against this" };
                    }

                }
                DataContext.Context.AddToRatings(new Rating
                {
                    Track = track,
                    User = user,
                    Value = -1
                });
                DataContext.Context.SaveChanges();
                return new Status { StatusCode = "Success", Message = "Track successfully voted against" };
            }
            return new Status { StatusCode = "Error", Message = "Track unknown" };
        }
        #endregion
    }
}
