using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Lib.Interfaces
{
    public interface ICoverGrabber
    {
        string GetCoverForTrack(Track track);
    }
}
