﻿using System;
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
using Spoffice.Website.Helpers;

namespace Spoffice.Website.Models.Output
{
    [XmlRoot("Track")]
    public class TrackOutput : BaseOutput
    {

        override protected string UrlType { get { return "track"; } }

        #region properties

        public string Title;
        public ArtistOutput Artist;
        public AlbumOutput Album;
        public int Length;
        public TrackState State;
        public string FormattedLength
        {
            get
            {
                return StringFormatter.MillisecondsToString(Length);
            }
        }
        #endregion
        public TrackOutput()
        {

        }
       
    }
}
