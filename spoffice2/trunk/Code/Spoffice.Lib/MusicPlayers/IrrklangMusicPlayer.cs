using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;
using System.Threading;
using IrrKlang;

namespace Spoffice.Lib.MusicPlayers
{
    public class IrrklangMusicPlayer : IMusicPlayer
    {
        #region IMusicPlayer Members

        private ISoundEngine engine;
        public IrrklangMusicPlayer()
        {
            engine = new ISoundEngine();
        }

        public void Play(Track track)
        {
            engine.StopAllSounds();
            track.State = TrackState.Playing;
            Thread thread = new Thread(new ParameterizedThreadStart(doPlay));
            thread.Start(track);
        }

        private void doPlay(object otrack)
        {
            Track track = otrack as Track;
            Thread.Sleep(1000);
            ISound sound = engine.Play2D(track.FilePath);
            while (!sound.Finished)
            {
                Thread.Sleep(500);
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
