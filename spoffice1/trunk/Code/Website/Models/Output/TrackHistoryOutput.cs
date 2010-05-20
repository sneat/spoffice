using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models.Output
{
    public class TrackHistoryOutput
    {
        public TrackOutput Track
        {
            get;
            set;
        }
        public long Timestamp
        {
            get;
            set;
        }
    }
}
