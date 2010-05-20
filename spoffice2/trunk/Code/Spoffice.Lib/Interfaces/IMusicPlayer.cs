using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Lib.Interfaces
{
    public enum PlayerState
    {
        Ready,
        Playing,
        Stopped,
        Paused,
        Error
    }
    public interface IMusicPlayer
    {
        void Play(Track track);
        void Stop();
        void Pause();
        void Resume();
    }
}
