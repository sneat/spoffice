using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Spotify.MetadataApi;

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

        public string GetCoverPath(TrackNode track)
        {
            foreach (ICoverGrabber grabber in grabbers)
            {
                string cover = grabber.GetCoverPath(track);
                if (!String.IsNullOrEmpty(cover))
                {
                    return cover;
                }
            }
            throw new Exception("Cover not found. Consider adding a cover grabber that will always return a default image");
        }

        public string GetCoverPath(AlbumNode album)
        {
            foreach (ICoverGrabber grabber in grabbers)
            {
                string cover = grabber.GetCoverPath(album);
                if (!String.IsNullOrEmpty(cover))
                {
                    return cover;
                }
            }
            throw new Exception("Cover not found. Consider adding a cover grabber that will always return a default image");
        }

        #endregion
    }
}
