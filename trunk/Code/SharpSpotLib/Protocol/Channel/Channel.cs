using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Enums;
using SharpSpotLib.Util;

namespace SharpSpotLib.Protocol.Channel
{
    internal class Channel
    {
        #region fields

        private static Int32 _nextId = 0;
        private static Dictionary<Int32, Channel> _channels = new Dictionary<Int32, Channel>();

        private IChannelListener _listener;

        #endregion


        #region properties

        public Int32 Id { get; private set; }
        public String Name { get; private set; }
        public ChannelState State { get; private set; }
        public ChannelType Type { get; private set; }
        public Int32 HeaderLength { get; private set; }
        public Int32 DataLength { get; private set; }

        #endregion


        #region methods

        private static void HandleException(Exception ex)
        {
            //FIXME: Do nothing?
        }

        public static void Register(Channel channel)
        {
		    Channel._channels.Add(channel.Id, channel);
	    }
    	
	    public static void Unregister(Int32 id)
        {
		    Channel._channels.Remove(id);
	    }

        public static void Process(Byte[] payload)
        {
            Channel channel;
            Int32 offset = 0;
            Int32 length = payload.Length;
            Int32 headerLength = 0;
            Int32 consumedLength = 0;

            /* Get Channel by id from payload. */
            if ((channel = Channel._channels[ShortUtilities.BytesToUnsignedShort(payload)]) == null)
            {
                HandleException(new Exception("Channel not found."));
                return;
            };

            offset += 2;
            length -= 2;

            if (channel.State == ChannelState.STATE_HEADER)
            {
                if (length < 2)
                {
                    HandleException(new Exception("Length is smaller than 2."));
                    return;
                }

                while (consumedLength < length)
                {
                    /* Extract length of next data. */
                    headerLength = ShortUtilities.BytesToUnsignedShort(payload, offset);

                    offset += 2;
                    consumedLength += 2;

                    if (headerLength == 0)
                    {
                        break;
                    }

                    if (consumedLength + headerLength > length)
                    {
                        HandleException(new Exception("Not enough data."));
                        return;
                    }

                    if (channel._listener != null)
                    {
                        Byte[] buffer = new Byte[headerLength];
                        Array.Copy(payload, offset, buffer, 0, headerLength);
                        channel._listener.ChannelHeader(channel, buffer);
                    }

                    offset += headerLength;
                    consumedLength += headerLength;

                    channel.HeaderLength += headerLength;
                }

                if (consumedLength != length)
                {
                    HandleException(new Exception("Didn't consume all data."));
                    return;
                }

                /* Upgrade state if this was the last (zero size) header. */
                if (headerLength == 0)
                {
                    channel.State = ChannelState.STATE_DATA;
                }

                return;
            }

            /*
             * Now we're either in the CHANNEL_DATA or CHANNEL_ERROR state.
             * If in CHANNEL_DATA and length is zero, switch to CHANNEL_END,
             * thus letting the callback routine know this is the last packet.
             */
            if (length == 0)
            {
                channel.State = ChannelState.STATE_END;

                if (channel._listener != null)
                {
                    channel._listener.ChannelEnd(channel);
                }
            }
            else
            {
                if (channel._listener != null)
                {
                    Byte[] buffer = new Byte[length];
                    Array.Copy(payload, offset, buffer, 0, length);
                    channel._listener.ChannelData(channel, buffer);
                }
            }

            channel.DataLength += length;

            /* If this is an AES key channel, force end state. */
            if (channel.Type == ChannelType.TYPE_AESKEY)
            {
                channel.State = ChannelState.STATE_END;

                if (channel._listener != null)
                {
                    channel._listener.ChannelEnd(channel);
                }
            }
        }

        public static void Error(Byte[] payload)
        {
            Channel channel;

            /* Get Channel by id from payload. */
            if ((channel = Channel._channels[ShortUtilities.BytesToUnsignedShort(payload)]) == null)
            {
                HandleException(new Exception("Channel not found."));
                return;
            };

            if (channel._listener != null)
            {
                channel._listener.ChannelError(channel);
            }

            Channel._channels.Remove(channel.Id);
        }

        #endregion


        #region construction

        public Channel(String name, ChannelType type, IChannelListener listener)
        {
            this.Id = Channel._nextId++;
            this.Name = name + "-" + this.Id;
            this.State = ChannelState.STATE_HEADER;
            this.Type = type;
            this.HeaderLength = 0;
            this.DataLength = 0;
            this._listener = listener;

            /* Force data state for AES key channel. */
            if (this.Type == ChannelType.TYPE_AESKEY)
                this.State = ChannelState.STATE_DATA;
        }

        #endregion
    }
}
