using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lastfm;
using Lastfm.Radio;
using WMPLib;
using System.Timers;
using System.Threading;
using System.IO;

namespace Spoffice.Lib
{

    public class Controller
    {
        public Station Station;

        private WindowsMediaPlayer player;
        private System.Timers.Timer timer;

        public List<Track> UpcomingTracks = new List<Track>();
        public Track CurrentTrack;

        private bool songEnded;

        public string PlayerState
        {
            get
            {
                return player.status;
            }
        }

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
            StringBuilder builder = new StringBuilder();
            StringWriter writer = new StringWriter(builder);

            WebSocketServer server = new WebSocketServer(8181, "http://localhost:49226/", "ws://localhost:8181/service");
            server.Logger = writer;
            server.LogLevel = ServerLogLevel.Verbose;
            server.ClientConnected += new ClientConnectedEventHandler(OnClientConnected);
            server.Start();
            Thread ServerThread = new Thread(new ThreadStart(KeepAlive));
            ServerThread.Start();

            player = new WindowsMediaPlayer();
            player.PlayStateChange += new _WMPOCXEvents_PlayStateChangeEventHandler(player_PlayStateChange);

            fetchMoreTracks();
            playNextTrack();

            timer = new System.Timers.Timer(1000);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

        }
        void OnClientConnected(WebSocketConnection sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Client connected");
            sender.Disconnected += new WebSocketDisconnectedEventHandler(OnClientDisconnected);
            sender.DataReceived += new DataReceivedEventHandler(OnClientMessage);
        }
        void OnClientMessage(WebSocketConnection sender, DataReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Client message");
        }
        void OnClientDisconnected(WebSocketConnection sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Client disconnected");

        }
        private void KeepAlive()
        {
            while (true)
            {
            }
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
            CurrentTrack = UpcomingTracks.FirstOrDefault();
            if (CurrentTrack != null)
            {
                UpcomingTracks.Remove(CurrentTrack);
                player.controls.stop();
                player.URL = CurrentTrack.StreamPath;
                player.controls.play();
            }
            if (UpcomingTracks.Count < 1)
            {
                fetchMoreTracks();
            }
        }
        private void fetchMoreTracks()
        {
            foreach (Track track in Station.FetchTracks(true, true))
            {
                UpcomingTracks.Add(track);
            }
        }

        // just forces there to be a new controller. doesnt do anything with it.
        public static void Start()
        {
            Controller c = Current;
        }
    }
}
