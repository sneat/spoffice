using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Spoffice.Website.Models.Spotify.MetadataApi;

namespace Spoffice.Website.Models
{
    public class TrackHistoryList
    {
        [XmlIgnore]
        [JsonIgnore]
        public List<TrackHistory> TrackHistories
        {
            get;
            set;
        }
        public List<TrackHistoryItem> History
        {
            get
            {
                List<TrackHistoryItem> tmp = new List<TrackHistoryItem>();
                foreach (TrackHistory history in TrackHistories)
                {
                    tmp.Add(new TrackHistoryItem(history));
                }
                return tmp;
            }
        }
    }
    public class TrackHistoryItem
    {
        public TrackNode Track
        {
            get;
            set;
        }
        public long Timestamp
        {
            get;
            set;
        }
        public TrackHistoryItem(TrackHistory history)
        {
            Timestamp = history.Datetime.Ticks;
            Track = new TrackNode
            {
                PrivateId = history.Track.Id,
                Title = history.Track.Title,
                Length = history.Track.Length
            };
            if (history.Track.Album != null){
                Track.Album = new AlbumNode{
                    PrivateId = history.Track.Album.Id,
                    Name = history.Track.Album.Name
                };
            }
            if (history.Track.Artist != null)
            {
                Track.Artist = new ArtistNode
                {
                    PrivateId = history.Track.Artist.Id,
                    Name = history.Track.Artist.Name
                };
            }
        }
    }
}
