using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;
using System.Collections.Generic;
using SharpSpotLib.Media;
using SharpSpotLib.Util;
using System.Xml.Serialization;
namespace Spoffice.Website.Models.Spotify.MetadataApi
{
    [XmlRoot("Artist")]
    public class ArtistNode : FeedNode
    {
        override protected string UrlType { get { return "artist"; } }
        override protected string LookupUrl { get { return "http://ws.spotify.com/lookup/1/?uri={0}&extras=albumdetail"; } }
        override protected string RootNode { get { return "artist"; } }

        #region properties
        public String Name;
        [XmlArrayItem(ElementName = "Album")] 
        public List<AlbumNode> Albums;
        #endregion

        #region constructors
        public ArtistNode()
        {
        }
        public ArtistNode(XElement root)
            : base(root)
        {
        }
        public ArtistNode(String id)
            : base(id)
        {
        }
        #endregion

        override protected void ParseElement(XElement root)
        {
            ParseHref(root);
            ParseMusicBrainz(root);

            XElement xname = root.Element(MusicSearch.ns + "name");
            if (xname != null)
                Name = xname.Value;

            XElement xalbums = root.Element(MusicSearch.ns + "albums");
            if (xalbums != null)
            {
                var albums = from item in xalbums.Elements(MusicSearch.ns + "album")
                             select new AlbumNode(item);

                Albums = albums.ToList();
            }

        }
    }
}
