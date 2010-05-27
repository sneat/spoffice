using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lastfm;
using Lastfm.Radio;

namespace Spoffice.Lib
{

    public class Controller
    {
        public Track[] UpcomingTracks;
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
            string API_KEY = "054afe505306e6b2b99f0a8a31203f90";
            string API_SECRET = "170b6bc5ceb525370e010db0d69026eb";

            Session session = new Session(API_KEY, API_SECRET);

            session.Authenticate("coolpink-dev", Lastfm.Utilities.MD5("c00lp1nk"));

            Station station = new Station(StationURI.GetRecommended("coolpink-dev"), session);
            UpcomingTracks = station.FetchTracks(true, true);

        }

        // just forces there to be a new controller. doesnt do anything with it.
        public static void Start()
        {
            Controller c = Current;
        }
    }
}
