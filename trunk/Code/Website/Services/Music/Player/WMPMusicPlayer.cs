using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
using Spoffice.Website.Models;
using WMPLib;

namespace Spoffice.Website.Services.Music.Player
{
    public class WMPMusicPlayer : IMusicPlayer
    {
        public long TotalBytes { get; set; }
        private WindowsMediaPlayer engine;
        private Track _Track;
        private bool isPlaying = false;
        public WMPMusicPlayer(long bytesPlayed)
        {
            TotalBytes = bytesPlayed;
            engine = new WindowsMediaPlayer();
        }
        #region WMPMusicPlayer Members

        public void PlayTrack(Track track)
        {
            _Track = track;
            // stop any current sounds just to make sure we don't hurt peoples ears
            if (engine.playState == WMPPlayState.wmppsPlaying || engine.playState == WMPPlayState.wmppsBuffering)
            {
                engine.controls.stop();
            }

            track.Progress = 0;
            Thread thread = new Thread(new ParameterizedThreadStart(DoPlay));
            thread.Start(_Track);
        }

        #endregion

        public void DoPlay(object t)
        {
            Track track = (Track)t;

            // sleep for a second. don't ask me why. I forgot why.
            Thread.Sleep(1000);

            engine.URL = track.CachePath;
            engine.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);
            engine.MediaError += new WMPLib._WMPOCXEvents_MediaErrorEventHandler(Player_MediaError);
            engine.controls.play();
            isPlaying = true;
            long startTotalBytes = TotalBytes;
            while (engine.playState != WMPPlayState.wmppsStopped)
            {
                track.Progress = (int)engine.controls.currentPosition * 1000; // currentPosition is in seconds, we want ms.
                if (track.Length == 0 && engine.currentMedia.duration > 0)
                {
                    // Work out the track length and size. Done in an if statement because we don't have duration available right away
                    track.Length = (int)engine.currentMedia.duration * 1000;
                    TotalBytes = startTotalBytes + Convert.ToInt32(((double)track.BytesTotal) * ((double)track.Progress / (double)track.Length));
                }
                System.Threading.Thread.Sleep(500);
            }
        }

        private void Player_PlayStateChange(int NewState)
        {
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                // Finished playing the song!
                _Track.OnPlayed();
            }
        }

        private void Player_MediaError(object pMediaObject)
        {
            System.Diagnostics.Debug.WriteLine("Cannot play media file.");
            // Do something to track that there was an error and remove the song from being played again in future
            _Track.OnPlayed();
        }


        public void Stop()
        {
            engine.controls.stop();
        }

    }
}
