using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;
using System.Threading;

namespace Spoffice.Lib.MusicPlayers
{
    public class IrrklangMusicPlayer : IMusicPlayer
    {
        #region IMusicPlayer Members

        public void Play(Track track)
        {
            track.State = TrackState.Playing;
            Thread thread = new Thread(new ParameterizedThreadStart(doPlay));
            thread.Start(track);
        }

        private void doPlay(object otrack)
        {
            Track track = otrack as Track;
            int i = 0;
            while (i++ < 100)
            {
                Thread.Sleep(100);
            }
            track.State = TrackState.Played;
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
