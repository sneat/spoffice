using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WMPLib;
using Spoffice.Lib.Interfaces;
using System.Threading;

namespace Spoffice.Lib.MusicPlayers
{
    public class WMPMusicPlayer : IMusicPlayer
    {
        private WindowsMediaPlayer engine;
        public WMPMusicPlayer()
        {
            engine = new WindowsMediaPlayer();
        }

        #region IMusicPlayer Members

        public void Play(Track track)
        {
            track.State = TrackState.Playing;
            if (engine.playState == WMPPlayState.wmppsPlaying || engine.playState == WMPPlayState.wmppsBuffering)
            {
                engine.controls.stop();
            }
            Thread thread = new Thread(new ParameterizedThreadStart(doPlay));
            thread.Start(track);
        }

        private void doPlay(object t)
        {
            Track track = t as Track;
            Thread.Sleep(1000);
            engine.URL = track.FilePath;
            while (engine.playState != WMPPlayState.wmppsStopped)
            {
                System.Threading.Thread.Sleep(500);
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
