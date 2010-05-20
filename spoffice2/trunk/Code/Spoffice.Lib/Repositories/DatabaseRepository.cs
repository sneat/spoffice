using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;

namespace Spoffice.Lib.Repositories
{
    public class DatabaseRepository : ITrackRepository
    {
        #region ITrackRepository Members

        public List<Track> GetTracks(int count)
        {
            return new List<Track>();
        }

        public void RemoveInvalidTrack(Track track)
        {
        }

        #endregion
    }
}
