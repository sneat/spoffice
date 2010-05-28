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

        public string currentTrack;
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

            // fill up our playlist with new tracks!
            fillPlaylist();

            currentTrack = player.currentMedia.sourceURL;

            player.controls.play();

        }
        private void fillPlaylist()
        {
            System.Diagnostics.Debug.WriteLine("fill");
            // get the list again.
            Track[] tracks = Station.FetchTracks(true, true);

            // if theres a track in there
            foreach (Track track in tracks){

                System.Diagnostics.Debug.WriteLine("adding track = " + track.StreamPath);
                WMPLib.IWMPMedia media = player.newMedia(track.StreamPath);
                player.currentPlaylist.appendItem(media);
                
            }
        }
        private void Player_PlayStateChange(int NewState)
        {

            currentTrack = player.currentMedia.sourceURL;

            // write out the current track url
            System.Diagnostics.Debug.WriteLine("track url = "+currentTrack);

            // write out the state
            System.Diagnostics.Debug.WriteLine("state = "+NewState.ToString());

            // write out the length of the playlist
            System.Diagnostics.Debug.WriteLine("playlist length = "+player.currentPlaylist.count.ToString());

            // write out the length of the playlist
            System.Diagnostics.Debug.WriteLine("playlist length = " + player.currentPlaylist.count.ToString());

            if (player.currentPlaylist.count < 2)
            {
                fillPlaylist();
            }
        }

        // just forces there to be a new controller. doesnt do anything with it.
        public static void Start()
        {
            Controller c = Current;
        }
    }
}
