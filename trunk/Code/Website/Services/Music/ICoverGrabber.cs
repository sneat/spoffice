using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Website.Models.Spotify.MetadataApi;

namespace Spoffice.Website.Services.Music
{
    public interface ICoverGrabber
    {
        string GetCoverPath(TrackNode track);
        string GetCoverPath(AlbumNode album);
    }
}
