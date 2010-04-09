using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Website.Models;

namespace Spoffice.Website.Services
{
    public interface IMusicPlayer
    {
        long TotalBytes { get; set; }
        void PlayTrack(Track track);
        void Stop();
    }
}
