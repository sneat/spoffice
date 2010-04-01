using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models.Output
{
    public class PlayerStatusOutput
    {
        public long PlayerPosition
        {
            get;
            set;
        }
        public long TotalBytes
        {
            get;
            set;
        }
        public List<TrackOutput> Tracks
        {
            get;
            set;
        }
    }
}
