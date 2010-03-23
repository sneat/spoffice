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

namespace Spoffice.Website.Models.Spotify.MetadataApi
{
    [XmlRoot("Album")]
    public class AlbumNode : FeedNode
    {

        override protected string UrlType { get { return "album"; } }
        override protected string LookupUrl { get { return "http://ws.spotify.com/lookup/1/?uri={0}&extras=trackdetail"; } }
        override protected string RootNode { get { return "album"; } }
     
        #region properties
        public String Name;
        public ArtistNode Artist;
        [XmlAttribute("available")]
        public bool IsAvailable = true;
        [XmlArrayItem(ElementName = "Track")] 
        public List<TrackNode> Tracks;
        [XmlIgnore]
        public string AlbumartCachePath
        {
            get
            {
                return String.Format("{0}Cache\\albumart\\{1}.jpg", HttpRuntime.AppDomainAppPath, PrivateId.ToString());
            }
        }
        [XmlIgnore]
        public string AlbumartCacheWebPath
        {
            get
            {
                return String.Format("~/Cache/albumart/{1}.jpg", PrivateId.ToString());
            }
        }
        #endregion

        public AlbumNode()
        {
        }
        public AlbumNode(string id)
            : base(id)
        {
        }
        public AlbumNode(XElement root)
            : base(root)
        {
        }

        #region methods
       
        override protected void ParseElement(XElement root)
        {
            ParseHref(root);
            ParseMusicBrainz(root);

            XElement xname = root.Element(MusicSearch.ns + "name");
            if (xname != null)
                Name = xname.Value;

            XElement xavailable = root.Element(MusicSearch.ns + "availability");
            if (xavailable != null){
                XElement xterritories = xavailable.Element(MusicSearch.ns + "territories");
                if (xterritories != null)
                    IsAvailable = xterritories.Value.Contains("GB");
            }

            XElement xartist = root.Element(MusicSearch.ns + "artist");
            if (xartist != null)
                Artist = new ArtistNode(xartist);
       

            XElement xtracks = root.Element(MusicSearch.ns + "tracks");
            if (xtracks != null)
            {
                var tracks = from item in xtracks.Elements(MusicSearch.ns + "track")
                             select new TrackNode(item);

                Tracks = tracks.ToList();
            }

        }
        #endregion
    }
}
