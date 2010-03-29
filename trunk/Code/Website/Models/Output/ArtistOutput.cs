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
using Spoffice.Website.Models.Spotify;
namespace Spoffice.Website.Models.Output
{
    [XmlRoot("Artist")]
    public class ArtistOutput : BaseOutput
    {
        override protected string UrlType { get { return "artist"; } }

        #region properties
        public String Name;
        [XmlArrayItem(ElementName = "Album")] 
        public List<AlbumOutput> Albums;
        #endregion

        #region constructors
        public ArtistOutput()
        {
        }

        #endregion

    }
}
