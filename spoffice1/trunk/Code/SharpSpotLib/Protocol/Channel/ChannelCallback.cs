using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;
using System.IO;

namespace SharpSpotLib.Protocol.Channel
{
    internal class ChannelCallback : IChannelListener
    {
        #region fields

        private Semaphore _done = new Semaphore(1); //FIXME: Dispose after use
        private List<ByteBuffer> _buffers = new List<ByteBuffer>();
        private Int32 _bytes = 0;

        #endregion


        #region properties

        public Boolean IsDone
        {
            get
            {
                return _done.AvailablePermits > 0;
            }
        }

        #endregion


        #region methods

        public Byte[] GetData()
        {
            ByteBuffer data = ByteBuffer.Allocate(this._bytes);
		
		    foreach (ByteBuffer b in this._buffers)
            {
			    data.Put(b.ToArray());
		    }
    		
		    /* Get data bytes. */
		    byte[] bytes = data.ToArray();
    		
		    /* Detect GZIP magic and return inflated data. */
		    if(bytes[0] == (byte)0x1f && bytes[1] == (byte)0x8b)
            {
			    return GZIP.Inflate(bytes);
		    }
    		
		    /* Return data. */
		    return bytes;
        }

        public Byte[] Get()
        {
            /* Wait for data to become available. */
            this._done.AcquireUninterruptibly();

            return this.GetData();
        }

        public Byte[] Get(TimeSpan timeout)
        {
            /* Wait for data to become available. */
            if (!this._done.TryAcquire(timeout))
            {
                throw new TimeoutException("Timeout while waiting for data.");
            }

            return this.GetData();
        }

        #endregion


        #region construction

        public ChannelCallback()
        {
            this._done.AcquireUninterruptibly();
        }

        #endregion


        #region ChannelListener Members

        public void ChannelHeader(Channel channel, Byte[] header)
        {
            /* Ignore */
        }

        public void ChannelData(Channel channel, byte[] data)
        {
            ByteBuffer buffer = new ByteBuffer(data);
            this._bytes += data.Length;
            this._buffers.Add(buffer);
        }

        public void ChannelError(Channel channel)
        {
            this._done.Release();
        }

        public void ChannelEnd(Channel channel)
        {
            Channel.Unregister(channel.Id);
            this._done.Release();
        }

        #endregion
    }
}
