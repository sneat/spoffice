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
using System.Xml.Serialization;
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
