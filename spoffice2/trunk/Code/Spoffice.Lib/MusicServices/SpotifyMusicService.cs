using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;
using System.Threading;

namespace Spoffice.Lib.MusicServices
{
    public class SpotifyMusicService : IMusicService
    {
        #region IMusicService Members

        public void Download(Track track)
        {
            track.State = TrackState.Buffering;
            Thread thread = new Thread(new ParameterizedThreadStart(doDownload));
            thread.Start(track);
        }

        #endregion

        private void doDownload(object otrack)
        {
            Track t = otrack as Track;
            int i = 0;
            while (i++ < 100)
            {
                Thread.Sleep(100);
            }
            t.State = TrackState.Buffered;
        }
    }
}
