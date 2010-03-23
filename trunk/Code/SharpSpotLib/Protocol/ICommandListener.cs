using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Protocol
{
    internal interface ICommandListener
    {
        void CommandReceived(Int32 command, Byte[] payload);
    }
}
