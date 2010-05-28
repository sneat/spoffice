﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lastfm;
using Lastfm.Radio;
using WMPLib;
using System.Timers;

namespace Spoffice.Lib
{

    public class Controller
    {
        public Station Station;

        private WindowsMediaPlayer player;
        private Timer timer;

        private List<Track> tracks = new List<Track>();
        public Track CurrentTrack;

        private bool songEnded;

        private static Controller current;
        public static Controller Current
        {
            get
            {
                return current ?? (current = new Controller());
            }
        }
        protected Controller()
        {
            // ok, as soon as a controller is created (i.e. when you start the app)...
            string API_KEY = "054afe505306e6b2b99f0a8a31203f90";
            string API_SECRET = "170b6bc5ceb525370e010db0d69026eb";

            // create a session and authenticate
            Session session = new Session(API_KEY, API_SECRET);

            // these will go in the web config
            session.Authenticate("coolpink-dev", Lastfm.Utilities.MD5("c00lp1nk"));

            // create a new station..
            Station = new Station(StationURI.GetRecommended("coolpink-dev"), session);

            fetchMoreTracks();

            player = new WindowsMediaPlayer();
            player.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(player_PlayStateChange);

            playNextTrack();

            timer = new Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
            timer.Start();

        }
        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (songEnded)
            {
                playNextTrack();
                songEnded = false;
                timer.Stop();
            }
        }
        private void player_PlayStateChange(int NewState)
        {
            switch (player.playState)
            {
                case WMPLib.WMPPlayState.wmppsMediaEnded:
                    songEnded = true;
                    timer.Start();
                    break;
                default:
                    break;
            }
        }
        private void playNextTrack()
        {
            if (tracks.Count < 2)
            {
                fetchMoreTracks();
            }
            CurrentTrack = tracks.FirstOrDefault();
            tracks.Remove(CurrentTrack);
            player.controls.stop();
            player.URL = CurrentTrack.StreamPath;
            player.controls.play();
        }
        private void fetchMoreTracks()
        {
            foreach (Track track in Station.FetchTracks(true, true))
            {
                tracks.Add(track);
            }
        }

        // just forces there to be a new controller. doesnt do anything with it.
        public static void Start()
        {
            Controller c = Current;
        }
    }
}
