using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
using SharpSpotLib.Util;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;

namespace Spoffice.Website.Models.Output
{
    public abstract class BaseOutput
    {
        [XmlIgnore]
        [JsonIgnore]
        virtual protected string UrlType { get { return ""; } }
        [XmlAttribute("publicid")]
        public string PublicId;
        [XmlAttribute("musicbrainz")]
        public Guid MusicBrainzId;
        [XmlIgnore]
        [JsonIgnore]
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
        public static Guid ConvertPublicToPrivate(string publicId)
        {
            return new Guid(SpotifyURI.ToHex(publicId));
        }
        public static string ConvertPrivateToPublic(Guid privateId)
        {
            return SpotifyURI.ToBase62(privateId.ToString("N"));
        }
    }
}
