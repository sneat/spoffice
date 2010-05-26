using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace Spoffice.Lib.Repositories
{
    public class XmlRepository : ITrackRepository
    {
        List<Track> tracks = new List<Track>();
        public XmlRepository(string path)
        {
            XDocument doc = XDocument.Load(path);
            foreach (XElement track in doc.Descendants("Track"))
            {
                tracks.Add(new Track
                {
                    Id = new Guid(track.Attribute("Id").Value),
                    Title = track.Element("Title").Value,
                    Album = track.Element("Album").Value,
                    Artist = track.Element("Artist").Value,
                    MusicService = track.Element("MusicService").Value
                });
            }
        }

        #region ITrackRepository Members

        List<Track> ITrackRepository.GetTracks(int count)
        {
            List<Track> ts = (from t in tracks orderby Guid.NewGuid() select t).Take(count).ToList();

            // reset the state
            foreach (Track t in ts)
            {
                t.State = TrackState.Empty;
            }
            return ts;
        }

        void ITrackRepository.RemoveInvalidTrack(Track track)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
