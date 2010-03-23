using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using SharpSpotLib.Media;
using SharpSpotLib.Protocol.Channel;
using SharpSpotLib.Enums;

namespace SharpSpotLib.Cache
{
    internal class SubstreamCache : FileCache
    {
        #region methods

        public String Hash(Media.File file, Int32 offset, Int32 length)
        {
            return file.Id + "/" + offset + "-" + length;
        }

        public void Load(String category, String hash, IChannelListener listener)
        {
            /* Load data in a separate thread, because we're an asynchronous load method. */

            new Thread(delegate() { 
                Channel channel = new Channel("Cached-Substream-Channel", ChannelType.TYPE_SUBSTREAM, null);
                listener.ChannelHeader(channel, null);
				listener.ChannelData(channel, Load(category, hash));
				listener.ChannelEnd(channel);
            }).Start();
	    }

        #endregion

        #region construction

        public SubstreamCache() : base()
        {
        }

        public SubstreamCache(String directory) : base(directory)
        {
        }

        #endregion
    }
}
