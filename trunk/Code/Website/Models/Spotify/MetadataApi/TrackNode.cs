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
using Spoffice.Website.Helpers;

namespace Spoffice.Website.Models.Spotify.MetadataApi
{
    [XmlRoot("Track")]
    public class TrackNode : FeedNode
    {

        override protected string UrlType { get { return "track"; } }
        override protected string LookupUrl { get { return "http://ws.spotify.com/lookup/1/?uri={0}"; } }
        override protected string RootNode { get { return "track"; } }

        #region properties

        public string Title;
        public ArtistNode Artist;
        public AlbumNode Album;
        public int Length;
        public string FormattedLength
        {
            get
            {
                return StringFormatter.MillisecondsToString(Length);
            }
        }
        #endregion
        public TrackNode()
        {

        }
        public TrackNode(string id)
            : base(id)
        {
        }
        public TrackNode(XElement root)
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
                Title = xname.Value;

            XElement xartist = root.Element(MusicSearch.ns + "artist");
            if (xartist != null)
                Artist = new ArtistNode(xartist);

            XElement xalbum = root.Element(MusicSearch.ns + "album");
            if (xalbum != null)
                Album = new AlbumNode(xalbum);

            XElement xlength = root.Element(MusicSearch.ns + "length");
            if (xlength != null)
                Length = (int)(Convert.ToDouble(xlength.Value) * 1000);

        }
        #endregion
    }
}
