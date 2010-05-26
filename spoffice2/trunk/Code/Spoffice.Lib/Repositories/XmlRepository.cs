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
        private string xmlpath;
        public XmlRepository(string path)
        {
            xmlpath = path;
            tracks = getAllTracksFromXml();
        }

        private List<Track> getAllTracksFromXml()
        {
            XDocument doc = XDocument.Load(xmlpath);
            List<Track> newTracks = new List<Track>();
            foreach (XElement track in doc.Descendants("Track"))
            {
                newTracks.Add(new Track
                {
                    Id = new Guid(track.Attribute("Id").Value),
                    Title = track.Element("Title").Value,
                    Album = track.Element("Album").Value,
                    Artist = track.Element("Artist").Value,
                    MusicService = track.Element("MusicService").Value,
                    FilePath = track.Element("FilePath").Value                    
                });
            }
            return newTracks;
        }

        #region ITrackRepository Members

        List<Track> ITrackRepository.GetTracks(int count)
        {
            return (from t in getAllTracksFromXml() orderby Guid.NewGuid() select t).Take(count).ToList();
        }

        void ITrackRepository.RemoveInvalidTrack(Track track)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
