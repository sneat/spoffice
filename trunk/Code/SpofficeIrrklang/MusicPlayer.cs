using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using IrrKlang;
using Spoffice.Website.Models;
using Spoffice.Website.Services;

namespace Spoffice.Irrklang
{
    public class MusicPlayer : IMusicPlayer
    {
        public long TotalBytes { get; set; }
        private ISoundEngine engine;
        public MusicPlayer()
        {
            engine = new ISoundEngine();
        }
        #region IMusicPlayer Members

        public void PlayTrack(Track track)
        {
            // stop any current sounds just to make sure we don't hurt peoples ears
            engine.StopAllSounds();

            track.Progress = 0;
            Thread thread = new Thread(new ParameterizedThreadStart(DoPlay));
            thread.Start(track);
        }

        public void Stop()
        {
            engine.StopAllSounds();
        }

        #endregion

        public void DoPlay(object t)
        {
            Track track = (Track)t;

            // sleep for a second. don't ask me why. I forgot why.
            Thread.Sleep(1000);

            ISound sound = engine.Play2D(track.CachePath);
            track.Length = (int)sound.PlayLength;
            long startTotalBytes = TotalBytes;
            while (!sound.Finished)
            {
                track.Progress = (int)sound.PlayPosition;
                TotalBytes = startTotalBytes + Convert.ToInt32(((double)track.BytesTotal) * ((double)sound.PlayPosition / (double)sound.PlayLength));
                System.Threading.Thread.Sleep(500);
            }

            track.OnPlayed();
        }
    }
}
