using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        private TrackState state = TrackState.Empty;
        public TrackState State
        {
            get
            {
                return state;
            }
            set
            {
                OnChangeState(this);
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
