using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Spoffice.Lib
{
    public enum TrackState
    {
        Empty,
        Buffering,
        Buffered,
        Playing,
        Played,
        Invalid
    }
    public class Track
    {

        public delegate void TrackChangeStateDelegate(Track track);
        public event TrackChangeStateDelegate OnChangeState;

        public Guid Id
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string Album
        {
            get;
            set;
        }
        public string Artist
        {
            get;
            set;
        }

        public string StateString
        {
            get
            {
                switch (state)
                {
                    case TrackState.Buffered:
                        return "buffered";
                    case TrackState.Buffering:
                        return "buffering";
                    case TrackState.Empty:
                        return "empty";
                    case TrackState.Invalid:
                        return "invalid";
                    case TrackState.Played:
                        return "played";
                    case TrackState.Playing:
                        return "playing";
                }
                return String.Empty;
            }
        }
        private TrackState state = TrackState.Empty;
        public TrackState State
        {
            get
            {
                return state;
            }
            set
            {
                //OnChangeState(this);
                state = value;
            }
        }
        public string MusicService
        {
            get;
            set;
        }

    }
}
