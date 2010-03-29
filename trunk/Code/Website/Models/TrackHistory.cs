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
            TrackHistoryOutput output = new TrackHistoryOutput();
            output.Timestamp = Datetime.Ticks;
            output.Track = Track.AsOutput();
            return output;
        }
    }
}
