using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lastfm;
using Lastfm.Radio;
using System.Threading;
using WMPLib;

namespace Spoffice.Lib
{

    public class Controller
    {
        public List<Track> UpcomingTracks = new List<Track>();
        public Station Station;
        private WindowsMediaPlayer player;

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

            // create a new player
            player = new WindowsMediaPlayer();

            // add an event listener
            player.PlayStateChange += new WMPLib._WMPOCXEvents_PlayStateChangeEventHandler(Player_PlayStateChange);

            // play track
            playUpcomingTrack();

        }
        private void playUpcomingTrack()
        {
            // get the list again.
            UpcomingTracks = Station.FetchTracks(true, true).ToList();
            // if theres a track in there
            if (UpcomingTracks.Count > 0)
            {

                // get the first
                Track trackToPlay = UpcomingTracks.FirstOrDefault();

                // set the stream path
                player.URL = trackToPlay.StreamPath;

                // play it
                player.controls.play();
            }
        }
        private void Player_PlayStateChange(int NewState)
        {
            // if a track has been played
            if ((WMPLib.WMPPlayState)NewState == WMPLib.WMPPlayState.wmppsStopped)
            {
                player.controls.stop();
                // play a new upcoming track
                playUpcomingTrack();
            }
        }

        // just forces there to be a new controller. doesnt do anything with it.
        public static void Start()
        {
            Controller c = Current;
        }
    }
}
