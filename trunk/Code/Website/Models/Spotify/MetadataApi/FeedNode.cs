using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using SharpSpotLib.Util;
using System.Xml.Serialization;
using System.IO;

namespace Spoffice.Website.Models.Spotify.MetadataApi
{
    public abstract class FeedNode
    {
        abstract protected string UrlType { get; }
        abstract protected string LookupUrl { get; }
        abstract protected string RootNode { get; }
        [XmlAttribute("publicid")]
        public string PublicId;
        [XmlAttribute("musicbrainz")]
        public Guid MusicBrainzId;
        [XmlIgnore]
        public Guid PrivateId
        {
            get
            {
                return ConvertPublicToPrivate(PublicId);
            }
            set
            {
                PublicId = ConvertPrivateToPublic(value);
            }
        }
    
        [XmlAttribute("uri")]
        public string URI
        {
            get
            {
                return String.Format("spotify:{0}:{1}", UrlType, PublicId);
            }
            set { }
        }
        protected string CachePath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(String.Format("~/{1}{0}{2}{0}{3}{0}{4}.xml", Path.DirectorySeparatorChar, "Cache", "requests", UrlType, PublicId));
            }
        }        
        public FeedNode()
        {
        }
        public FeedNode(XElement root)
        {
            ParseElement(root);
        }
        public FeedNode(string id)
        {
            PublicId = id;
            XDocument data;
            if (File.Exists(CachePath) && File.GetLastWriteTime(CachePath).AddMonths(1) > DateTime.Now)
            {
                data = XDocument.Load(CachePath);
            }
            else
            {
                data = XDocument.Load(String.Format(LookupUrl, URI));
                CreateFolder(CachePath);
                data.Save(CachePath);
            }
            ParseElement(data.Element(MusicSearch.ns + RootNode));
        }
        abstract protected void ParseElement(XElement root);
        protected void ParseHref(XElement root)
        {
            XAttribute xhref = root.Attribute("href");
            if (xhref != null)
                PublicId = new SpotifyURI(xhref.Value).Id;
        }
        protected void ParseMusicBrainz(XElement root)
        {
            try
            {
                XElement xmusicbrainz = root.Elements(MusicSearch.ns + "id").Where(x => x.Attribute("type").Value == "mbid").SingleOrDefault();
                if (xmusicbrainz != null)
                    MusicBrainzId = new Guid(xmusicbrainz.Value);
            }
            catch (Exception e)
            {
            }
        }
        public static Guid ConvertPublicToPrivate(string publicId)
        {
            return new Guid(SpotifyURI.ToHex(publicId));
        }
        public static string ConvertPrivateToPublic(Guid privateId)
        {
            return SpotifyURI.ToBase62(privateId.ToString("N"));
        }
        private static void CreateFolder(string path)
        {
            if (!Directory.GetParent(path).Exists)
            {
                CreateFolder(Directory.GetParent(path).FullName);
            }
            if (Path.GetFileName(path) != String.Empty)
            {
                path = new FileInfo(path).Directory.FullName;
            }
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}
