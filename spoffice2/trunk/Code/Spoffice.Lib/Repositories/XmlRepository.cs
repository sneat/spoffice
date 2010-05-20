using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;

namespace Spoffice.Lib.Repositories
{
    public class XmlRepository : ITrackRepository
    {
        public XmlRepository(string path)
        {

        }

        #region ITrackRepository Members

        public List<Track> GetTracks(int count)
        {
            throw new NotImplementedException();
        }

        public void RemoveInvalidTrack(Track track)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
