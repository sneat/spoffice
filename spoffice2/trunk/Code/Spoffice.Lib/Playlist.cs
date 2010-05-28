using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using WMPLib;
using System.Timers;

namespace Spoffice.Lib
{
    public class Playlist
    {
        public bool SongEnded = true;
        private Timer CheckSong;
        ArrayList SongsInPlaylist = new ArrayList();
        private int Index = 0;
        public WindowsMediaPlayer MediaPlayer;
        public Playlist(WindowsMediaPlayer Player)
        {
            MediaPlayer = Player;
            Index = 0;

            CheckSong = new Timer(1000);
            CheckSong.Elapsed += new ElapsedEventHandler(CheckSong_Elapsed);
            CheckSong.Start();
            MediaPlayer.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(MediaPlayer_PlayStateChange);
            Play();
        }

        void CheckSong_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (SongEnded)
            {
                NextSong();
                SongEnded = false;
                CheckSong.Stop();
            }
        }

        void MediaPlayer_PlayStateChange(int NewState)
        {
            switch (MediaPlayer.playState)
            {
                case WMPLib.WMPPlayState.wmppsMediaEnded:
                    SongEnded = true;
                    CheckSong.Start();
                    break;
                default:
                    break;
            }
        }

        public void AddSongs(string[] Songs)
        {
            for (int i = 0; i < Songs.Length; i++)
                AddSong(Songs[i]);
        }
        public void AddSong(string Song)
        {
            SongsInPlaylist.Add(Song);
        }
        public int Volume
        {
            set { MediaPlayer.settings.volume = value; }
            get { return MediaPlayer.settings.volume; }
        }
        public void DeleteSong(string Song)
        {
            if (Song == SongsInPlaylist[Index].ToString())
            {
                MediaPlayer.controls.stop();
                Index--;
            }
            SongsInPlaylist.Remove(Song);
            MediaPlayer.controls.play();
        }
        public void DeletePlaylist()
        {
            MediaPlayer.controls.stop();
            SongsInPlaylist.Clear();
            Index = 0;
        }
        public void Play()
        {
            if (SongsInPlaylist.Count > 0 && SongsInPlaylist[Index] != null)
                MediaPlayer.URL = SongsInPlaylist[Index].ToString();
        }
        public void Play(int Slot)
        {
            if (SongsInPlaylist[Slot - 1] != null)
                MediaPlayer.URL = SongsInPlaylist[Slot - 1].ToString();
        }
        public void Play(string name)
        {
            int slot = SongsInPlaylist.BinarySearch(name, null);
            if (slot >= 0 && slot < SongsInPlaylist.Count)
                MediaPlayer.URL = SongsInPlaylist[slot].ToString();
        }
        public void Pause()
        {
            MediaPlayer.controls.pause();
        }
        public void Stop()
        {
            MediaPlayer.controls.stop();
        }
        public void NextSong()
        {
            if (Index != SongsInPlaylist.Count - 1)
            {
                Index++;
                MediaPlayer.controls.stop();
                MediaPlayer.URL = SongsInPlaylist[Index].ToString();
                MediaPlayer.controls.play();
            }
            else
            {
                Index = 0;
                MediaPlayer.controls.stop();
                MediaPlayer.URL = SongsInPlaylist[0].ToString();
                MediaPlayer.controls.play();
            }
        }
        public void PrevSong()
        {
            if (Index != 0)
            {
                Index--;
                MediaPlayer.controls.stop();
                MediaPlayer.URL = SongsInPlaylist[Index].ToString();
                MediaPlayer.controls.play();
            }
            else
            {
                Index = SongsInPlaylist.Count - 1;
                MediaPlayer.controls.stop();
                MediaPlayer.URL = SongsInPlaylist[Index].ToString();
                MediaPlayer.controls.play();
            }
        }
    }
}
