using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Enums
{
    internal enum ChannelState
    {
        STATE_HEADER,
        STATE_DATA,
        STATE_END,
        STATE_ERROR
    }
}
