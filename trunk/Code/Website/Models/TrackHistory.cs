using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models
{
    public partial class TrackHistory
    {
        
        public TrackHistoryOutput AsOutput()
        {
            long TicksToMillisOffset = (DateTime.UtcNow - new DateTime(1970, 1, 1)).Ticks;
            TrackHistoryOutput output = new TrackHistoryOutput();
            output.Timestamp = (Datetime.Ticks - TicksToMillisOffset)  / TimeSpan.TicksPerMillisecond;
            output.Track = Track.AsOutput();
            return output;
        }
    }
}
