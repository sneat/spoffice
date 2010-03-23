using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Protocol.Channel
{
    internal class ChannelAdapter : IChannelListener
    {
        #region ChannelListener Members

        public void ChannelHeader(Channel channel, Byte[] header)
        {
        }

        public void ChannelData(Channel channel, Byte[] data)
        {
        }

        public void ChannelError(Channel channel)
        {
        }

        public void ChannelEnd(Channel channel)
        {
        }

        #endregion
    }
}
