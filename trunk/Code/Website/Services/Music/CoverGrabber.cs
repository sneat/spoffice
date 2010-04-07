using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Services.Music
{
    public class CoverGrabber : ICoverGrabber
    {

        private List<ICoverGrabber> grabbers;
        #region ICoverGrabber Members

        public CoverGrabber(List<ICoverGrabber> grabbers)
        {
            this.grabbers = grabbers;
        }

        public string GetCoverPath(AlbumOutput album)
        {
            foreach (ICoverGrabber grabber in grabbers)
            {
                string cover = grabber.GetCoverPath(album);
                if (!String.IsNullOrEmpty(cover))
                {
                    return cover;
                }
            }
            return "/Content/blank_album.png";
        }

        #endregion
    }
}
