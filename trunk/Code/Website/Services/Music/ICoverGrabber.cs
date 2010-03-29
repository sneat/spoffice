using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Services.Music
{
    public interface ICoverGrabber
    {
        string GetCoverPath(TrackOutput track);
        string GetCoverPath(AlbumOutput album);
    }
}
