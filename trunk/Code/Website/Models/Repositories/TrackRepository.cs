using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models
{
    public class TrackRepository : ITrackRepository
    {
         #region ITrackRepository Members

        public void AddTrackToPlayHistory(Track track)
        {
            TrackHistory trackHistory = new TrackHistory();
            trackHistory.Track = track;
            trackHistory.Datetime = DateTime.Now;
            DataContext.Context.AddToTrackHistories(trackHistory);
            DataContext.Context.SaveChanges();
        }

        public void FullyRemoveTrack(Track track)
        {
            
            Favourite favourite = (from m in DataContext.Context.Favourites
                                   where m.Track.Id == track.Id
                                   select m).FirstOrDefault();
            if (favourite != null)
            {
                try
                {
                    DataContext.RatingRepository.DeleteRatingsForTrack(track);

                    DataContext.Context.DeleteObject(favourite);
                    DataContext.Context.SaveChanges();
                }
                catch (Exception e) { 
                
                }
            }
        }

        public List<Track> GetTracksToPlay(int maxCount)
        {
            return GetTracksToPlay(maxCount, new List<Guid>());
        }

        public List<Track> GetTracksToPlay(List<Guid> exclude)
        {
            return GetTracksToPlay(1, exclude);
        }

        public List<Track> GetTracksToPlay(int maxCount, List<Guid> exclude)
        {
            List<Track> tracks = DataContext.Context.GetTracksToPlay(maxCount, string.Join(",", exclude.Select(p => p.ToString()).ToArray())).ToList();
            foreach (Track track in tracks)
            {
                track.Artist = DataContext.TrackRepository.GetTrackById(track.Id).Artist;
                track.Album = DataContext.TrackRepository.GetTrackById(track.Id).Album;
            }
            return tracks;
        }

        #endregion

        #region ITrackRepository Members


        public Track GetTrackById(Guid id)
        {
            return DataContext.Context.Tracks.Include("Artist").Include("Album").Where(m => m.Id == id).FirstOrDefault();
        }


        #endregion
    }
}
