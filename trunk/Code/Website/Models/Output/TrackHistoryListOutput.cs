using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Spoffice.Website.Models.Output
{
    public class TrackHistoryListOutput
    {
        public TrackHistoryListOutput (List<TrackHistory> history){
            History = new List<TrackHistoryOutput>();
            foreach (TrackHistory item in history)
            {
                History.Add(item.AsOutput());
            }
        }
        public List<TrackHistoryOutput> History
        {
            get;
            set;
        }
    }
 
}
