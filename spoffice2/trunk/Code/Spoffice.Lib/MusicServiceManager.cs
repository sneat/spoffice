using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;

namespace Spoffice.Lib
{
    public class MusicServiceManager
    {
        private static Dictionary<string, IMusicService> Services = new Dictionary<string, IMusicService>();
        public static void AddService(string name, IMusicService service)
        {
            Services.Add(name, service);
        }
        public static IMusicService GetServiceForTrack(Track track)
        {
            return Services[track.MusicService];
        }
    }
}
