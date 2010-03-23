using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models
{
    public class TrackRepository : ITrackRepository
    {
        #region ITrackRepository Members

        public void AddTrackToPlayHistory(Track track)
        {
            //throw new NotImplementedException();
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
            spofficeEntities db = new spofficeEntities();
            return db.GetTracksToPlay(maxCount, string.Join(",", exclude.Select(p => p.ToString()).ToArray())).ToList();
        }

        public long GetTotalBytesPlayed()
        {
            long totalbytes = 0;
            spofficeEntities db = new spofficeEntities();
            var qry = from track in db.TrackHistories
                      group track by track.Track
                          into grp
                          select new
                          {
                              Track_Id = grp.Select(x => x.Track.Id).FirstOrDefault(),
                              Plays = grp.Select(x => x.Track).Distinct().Count()
                          };
            int lostTracks = 0;
            List<long> bytes = new List<long>();
            foreach (var row in qry)
            {
                string path = String.Format(new Track { Id = row.Track_Id }.CachePath, HttpRuntime.AppDomainAppPath, row.Track_Id.ToString());

                if (System.IO.File.Exists(path))
                {
                    long length = new System.IO.FileInfo(path).Length;
                    totalbytes += length;
                    bytes.Add(length);
                }
                else
                {
                    lostTracks++;
                }
            }
            return bytes.Count > 0 ? totalbytes + ((long)bytes.Average() * lostTracks) : totalbytes + 4000 * 1024 * lostTracks;
        }

        #endregion
    }
}
