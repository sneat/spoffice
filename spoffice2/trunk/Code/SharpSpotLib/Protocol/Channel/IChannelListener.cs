using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Protocol.Channel
{
    internal interface IChannelListener
    {
        void ChannelHeader(Channel channel, Byte[] header);
        void ChannelData(Channel channel, Byte[] data);
        void ChannelError(Channel channel);
        void ChannelEnd(Channel channel);
    }
}
