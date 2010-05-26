using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;

namespace Spoffice.Lib
{
    public class MusicServiceManager
    {
        private static Dictionary<string, IMusicService> services = new Dictionary<string, IMusicService>();
        public static void AddService(string name, IMusicService service)
        {
            services.Add(name, service);
        }
        public static void DownloadTrack(Track track)
        {
            if (services.ContainsKey(track.MusicService))
            {
                services[track.MusicService].Download(track);
            }
            else
            {
                track.State = TrackState.Invalid;
            }
        }
    }
}
