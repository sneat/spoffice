using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Web;
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
        [XmlAttribute("musicbrainz")]

        public Guid MusicBrainzId;
        [XmlAttribute("id")]
        public Guid Id
        {
            get;
            set;
        } 
        protected string CachePath
        {
            get
            {
                return HttpContext.Current.Server.MapPath(String.Format("~/{1}{0}{2}{0}{3}{0}{4}.xml", Path.DirectorySeparatorChar, "Cache", "requests", UrlType, Id.ToString("N")));
            }
        } 
    }
}
