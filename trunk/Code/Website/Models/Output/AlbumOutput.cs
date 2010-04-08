using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Collections.Generic;
using SharpSpotLib.Media;
using SharpSpotLib.Util;
using Newtonsoft.Json;

namespace Spoffice.Website.Models.Output
{
    [XmlRoot("Album")]
    public class AlbumOutput : BaseOutput
    {

        override protected string UrlType { get { return "album"; } }
     
        #region properties
        public String Name;
        public ArtistOutput Artist;
        [XmlAttribute("available")]
        public bool IsAvailable = true;
        [XmlArrayItem(ElementName = "Track")] 
        public List<TrackOutput> Tracks;

        [XmlIgnore]
        [JsonIgnore]
        public string AlbumartCachePath
        {
            get
            {
                return String.Format("{0}Cache\\albumart\\{1}.jpg", HttpRuntime.AppDomainAppPath, Id.ToString());
            }
        }
        [XmlIgnore]
        [JsonIgnore]
        public string AlbumartCacheWebPath
        {
            get
            {
                return String.Format("~/Cache/albumart/{0}.jpg", Id.ToString());
            }
        }
        #endregion


    }
}
