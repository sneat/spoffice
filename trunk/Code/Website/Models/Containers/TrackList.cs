using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Spotify.MetadataApi;

namespace Spoffice.Website.Models
{
    public class TrackList
    {
        public List<TrackNode> Tracks
        {
            get;
            set;
        }
    }
}
