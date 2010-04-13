using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml.Linq;
using Spoffice.Website.Services;
using Spoffice.Website.Models.Output;

namespace Spoffice.LastFM
{
    public class CoverGrabber : ICoverGrabber
    {
        #region ICoverGrabber Members

        public string GetCoverPath(AlbumOutput album)
        {
            String xml_request_url = "http://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key=b25b959554ed76058ac220b7b2e0a026&artist={0}&album={1}";
            try
            {
                XElement root = XElement.Load(String.Format(xml_request_url, HttpUtility.UrlEncode(album.Artist.Name), HttpUtility.UrlEncode(album.Name)));
                if (root.Attribute("status").Value == "ok")
                {
                    return root.Descendants("image").Last().Value;
                }
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        #endregion
    }
}
