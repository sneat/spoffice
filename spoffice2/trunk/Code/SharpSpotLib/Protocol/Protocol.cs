//FIXME: Debug
//#define ADAPTER_MODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;
using System.Net;
using System.Net.Sockets;
using System.IO;
using SharpSpotLib.Exceptions;
using SharpSpotLib.Crypto;
using SharpSpotLib.Enums;
using SharpSpotLib.Protocol.Channel;
using SharpSpotLib.Media;
using System.Threading;

namespace SharpSpotLib.Protocol
{
    internal class Protocol
    {
        #region fields

        private TcpClient _channel = null;
        private HostnamePortPair _server = null;
        private Session _session;
        private List<ICommandListener> _listeners = new List<ICommandListener>();
        private List<HostnamePortPair> _allServers = null;

        #endregion


        #region properties

        public String CurrentServer
        {
            get
            {
                if (_server == null)
                    return null;
                return _server.Hostname;
            }
        }

        #endregion


        #region methods

        private void ShuffleServers(List<HostnamePortPair> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                HostnamePortPair value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        private void GetServers()
        {
            _allServers = DNS.LookupSRV("_spotify-client._tcp.spotify.com");
        }

        public void Connect()
        {
		    /* Lookup servers via DNS SRV query. */
		    //List<HostnamePortPair> servers = DNS.LookupSRV("_spotify-client._tcp.spotify.com");

            Thread dnsThread = new Thread(new ThreadStart(GetServers));
            dnsThread.IsBackground = true;
            dnsThread.Start();

            //10 sec timeout
            for (Int32 i = 0; i < 100; i++)
            {
                if (_allServers != null)
                    break;
                Thread.Sleep(100);
            }

            List<HostnamePortPair> servers = new List<HostnamePortPair>();
            if (_allServers != null)
                servers.AddRange(_allServers);
            dnsThread.Abort();
            _allServers = null;

		    /* Add a fallback servers if others don't work. */
		    servers.Add(new HostnamePortPair("ap.spotify.com", 80));
		    servers.Add(new HostnamePortPair("ap.spotify.com", 443));
            servers.Add(new HostnamePortPair("ap.spotify.com", 4070));

            //Shuffle servers, to pick them in random order
            ShuffleServers(servers);
    		
		    /* Try to connect to each server, stop trying when connected. */
		    foreach (HostnamePortPair server in servers)
            {
			    try
                {
				    /* Try to connect to current server with a timeout of 1 second. */

				    this._channel = new TcpClient();
                    this._channel.Connect(server.Hostname, server.Port); //FIXME: Add 1 sec timeout
    				
				    /* Save server for later use. */
				    this._server = server;
    				
				    break;
			    }
			    catch(Exception)
                {
			    }
		    }
    		
		    /* If connection was not established, return false. */
		    if(!this._channel.Client.Connected)
            {
			    throw new ConnectionException("Error connecting to any server.");
		    }
	    }

        public void Disconnect()
        {
		    try
            {
			    /* Close connection to server. */
			    this._channel.Close();
		    }
		    catch(Exception e)
            {
			    throw new ConnectionException("Error disconnecting from '" + this._server + "'!", e);
		    }
	    }

        public void AddListener(ICommandListener listener)
        {
            this._listeners.Add(listener);
        }

        public void SendInitialPacket()
        {
		    ByteBuffer buffer = ByteBuffer.Allocate(
			    2 + 2 + 4 + 4 + 4 + 4 + 4 + 4 + 4 + 16 + 96 + 128 + 1 + 1 + 2 + 0 + this._session.Username.Length + 1
		    );
    		
		    /* Append fields to buffer. */
		    buffer.PutShort((short)3); /* Version: 3 */
		    buffer.PutShort((short)0); /* Length (update later) */
            buffer.PutInt(0x00000300); /* Unknown */
		    buffer.PutInt(0x00030C00); /* Unknown */
		    buffer.PutInt(this._session.ClientRevision);
		    buffer.PutInt(0x00000000); /* Unknown */
		    buffer.PutInt(0x01000000); /* Unknown */
		    buffer.Put(this._session.ClientId); /* 4 bytes */
		    buffer.PutInt(0x00000000); /* Unknown */
		    buffer.Put(this._session.ClientRandom); /* 16 bytes */
		    buffer.Put(this._session.DHPublicKey.ToByteArray()); /* 96 bytes */
		    buffer.Put(this._session.RSAPublicKey.ToByteArray()); /* 128 bytes */
		    buffer.Put((byte)0); /* Random length */
		    buffer.Put((byte)this._session.Username.Length); /* Username length */
		    buffer.PutShort((short)0x0100); /* Unknown */
		    buffer.Put(this._session.UsernameBytes);
		    buffer.Put((byte)0x40); /* Unknown */
		    //buffer.put((byte)(0x00)); /* Unknown */
    		
		    /* Update length byte. */
		    buffer.PutShort(2, (short)buffer.Position);
		    buffer.Flip();
    		
		    /* Save initial client packet for auth hmac generation. */
		    this._session.InitialClientPacket = new Byte[buffer.Remaining];
    		
		    buffer.Get(this._session.InitialClientPacket);
		    buffer.Flip();
    		
		    /* Send it. */
		    this.Send(buffer);
	    }

        public void ReceiveInitialPacket()
        {
		    Byte[] buffer = new Byte[512];
		    Int32 ret, paddingLength, usernameLength;
    		
		    /* Save initial server packet for auth hmac generation. 1024 bytes should be enough. */
		    ByteBuffer serverPacketBuffer = ByteBuffer.Allocate(1024);
    		
		    /* Read server random (first 2 bytes). */
		    if ((ret = this.Receive(this._session.ServerRandom, 0, 2)) != 2)
			    throw new ProtocolException("Failed to read server random.");
    		
		    /* Check if we got a status message. */
		    if (this._session.ServerRandom[0] != 0x00){
			    /*
			     * Substatuses:
			     * 0x01    : Client upgrade required.
			     * 0x03    : Nonexistent user.
			     * 0x04    : Account has been disabled.
			     * 0x06    : You need to complete your account details.
			     * 0x09    : Your current country doesn't match that set in your profile.
			     * Default : Unknown error
			     */
			    StringBuilder message = new StringBuilder(255);
    			
			    /* Check substatus and set message. */
			    switch (this._session.ServerRandom[1])
                {
				    case 0x01:
					    message.Append("Client upgrade required: ");
					    break;
				    case 0x03:
					    message.Append("Nonexistent user.");
					    break;
				    case 0x04:
					    message.Append("Account has been disabled.");
					    break;
				    case 0x06:
					    message.Append("You need to complete your account details.");
					    break;
				    case 0x09:
					    message.Append("Your current country doesn't match that set in your profile.");
					    break;
				    default:
					    message.Append("Unknown error: " + this._session.ServerRandom[1].ToString());
					    break;
			    }
    			
			    /* If substatus is 'Client upgrade required', read and append upgrade URL. */
			    if(this._session.ServerRandom[1] == 0x01){
				    if((ret = this.Receive(buffer, 0x11a)) > 0){
					    paddingLength = buffer[0x119] & 0xFF;
    					
					    if ((ret = this.Receive(buffer, paddingLength)) > 0)
                        {
                            message.Append(Encoding.ASCII.GetString(buffer, 0, ret));
					    }
				    }
			    }
    			
			    throw new ProtocolException(message.ToString());
		    }
    		
		    /* Read server random (next 14 bytes). */
		    if((ret = this.Receive(this._session.ServerRandom, 2, 14)) != 14)
			    throw new ProtocolException("Failed to read server random.");
    		
		    /* Save server random to packet buffer. */
		    serverPacketBuffer.Put(this._session.ServerRandom);
    		
		    /* Read server public key (Diffie Hellman key exchange). */
		    if((ret = this.Receive(buffer, 96)) != 96)
			    throw new ProtocolException("Failed to read server public key.");
    		
		    /* Save DH public key to packet buffer. */
            Byte[] dhKey = new Byte[96];
            Array.Copy(buffer, dhKey, 96);
		    serverPacketBuffer.Put(dhKey);
    		
		    /* 
		     * Convert key, which is in raw byte form to a DHPublicKey
		     * using the DHParameterSpec (for P and G values) of our
		     * public key. Y value is taken from raw bytes.
		     */
            this._session.DHServerPublicKey = new DHPublicKey(dhKey);
    		
		    /* Read server blob (256 bytes). */
		    if((ret = this.Receive(this._session.ServerBlob, 0, 256)) != 256)
			    throw new ProtocolException("Failed to read server blob.");
    		
		    /* Save RSA signature to packet buffer. */
		    serverPacketBuffer.Put(this._session.ServerBlob);
    		
		    /* Read salt (10 bytes). */
		    if((ret = this.Receive(this._session.Salt, 0, 10)) != 10)
			    throw new ProtocolException("Failed to read salt.");
    		
		    /* Save salt to packet buffer. */
		    serverPacketBuffer.Put(this._session.Salt);
    		
		    /* Read padding length (1 byte). */
		    if((paddingLength = this.Receive()) == -1)
			    throw new ProtocolException("Failed to read paddling length.");
    		
		    /* Save padding length to packet buffer. */
		    serverPacketBuffer.Put((Byte)paddingLength);
    		
		    /* Check if padding length is valid. */
		    if(paddingLength <= 0)
			    throw new ProtocolException("Padding length is negative or zero.");
    		
		    /* Read username length. */
		    if((usernameLength = this.Receive()) == -1)
			    throw new ProtocolException("Failed to read username length.");
    		
		    /* Save username length to packet buffer. */
		    serverPacketBuffer.Put((Byte)usernameLength);
    		
		    /* Read lengths of puzzle challenge and unknown fields */
		    this.Receive(buffer, 8);
    		
		    /* Save bytes to packet buffer. */
		    serverPacketBuffer.Put(buffer, 0, 8);
    		
		    /* Get lengths of puzzle challenge and unknown fields.  */
		    ByteBuffer dataBuffer = ByteBuffer.Wrap(buffer, 0, 8);
		    Int32 puzzleChallengeLength = dataBuffer.GetShort();
		    Int32 unknownLength1 = dataBuffer.GetShort();
            Int32 unknownLength2 = dataBuffer.GetShort();
            Int32 unknownLength3 = dataBuffer.GetShort();
    		
		    /* Read padding. */
		    if((ret = this.Receive(buffer, paddingLength)) != paddingLength)
			    throw new ProtocolException("Failed to read padding.");
    		
		    /* Save padding (random bytes) to packet buffer. */
		    serverPacketBuffer.Put(buffer, 0, paddingLength);
    		
		    /* Read username into buffer and copy it to 'session.username'. */
		    if((ret = this.Receive(buffer, usernameLength)) != usernameLength)
			    throw new ProtocolException("Failed to read username.");
    		
		    /* Save username to packet buffer. */
		    serverPacketBuffer.Put(buffer, 0, usernameLength);
    		
		    /* Save username to session. */
            Byte[] usernameBuffer = new Byte[usernameLength];
            Array.Copy(buffer, usernameBuffer, usernameLength);
            this._session.UsernameBytes = usernameBuffer;
    		
		    /* Receive puzzle challenge and unknown bytes. */
		    this.Receive(buffer, 0, puzzleChallengeLength);
		    this.Receive(buffer, puzzleChallengeLength, unknownLength1);
		    this.Receive(buffer, puzzleChallengeLength + unknownLength1, unknownLength2);
		    this.Receive(buffer, puzzleChallengeLength + unknownLength1 + unknownLength2, unknownLength3);
    		
		    /* Save to packet buffer. */
		    serverPacketBuffer.Put(buffer, 0, puzzleChallengeLength + unknownLength1 + unknownLength2 + unknownLength3);
		    serverPacketBuffer.Flip();
    		
		    /* Write data from packet buffer to byte array. */
		    this._session.InitialServerPacket = new Byte[serverPacketBuffer.Remaining];
    		
		    serverPacketBuffer.Get(this._session.InitialServerPacket);
    		
		    /* Wrap buffer in order to get values. */
		    dataBuffer = ByteBuffer.Wrap(buffer, 0, puzzleChallengeLength + unknownLength1 + unknownLength2 + unknownLength3);
    		
		    /* Get puzzle denominator and magic. */
		    if(dataBuffer.Get() == 0x01)
            {
			    this._session.PuzzleDenominator = dataBuffer.Get();
			    this._session.PuzzleMagic = dataBuffer.GetInt();
		    }
		    else
			    throw new ProtocolException("Unexpected puzzle challenge.");
	    }

        public void SendAuthenticationPacket()
        {
		    ByteBuffer buffer = ByteBuffer.Allocate(20 + 1 + 1 + 4 + 2 + 15 + 8);
    		
		    /* Append fields to buffer. */
		    buffer.Put(this._session.AuthHmac); /* 20 bytes */
		    buffer.Put((Byte)0); /* Random data length */
		    buffer.Put((Byte)0); /* Unknown. */
		    buffer.PutShort((Int16)this._session.PuzzleSolution.Length);
		    buffer.PutInt(0x0000000); /* Unknown. */
		    //buffer.put(randomBytes); /* Zero random bytes :-) */
		    buffer.Put(this._session.PuzzleSolution); /* 8 bytes */
		    buffer.Flip();
    		
		    /* Send it. */
		    this.Send(buffer);
	    }

        public void ReceiveAuthenticationPacket()
        {
		    Byte[] buffer = new Byte[512];
		    Int32 payloadLength;
    		
		    /* Read status and length. */
		    if(this.Receive(buffer, 2) != 2)
			    throw new ProtocolException("Failed to read status and length bytes.");
    		
		    /* Check status. */
		    if(buffer[0] != 0x00)
			    throw new ProtocolException("Authentication failed! (Error " + buffer[1] + ")");
    		
		    /* Check payload length. AND with 0x00FF so we don't get a negative integer. */
		    if((payloadLength = buffer[1] & 0xFF) <= 0)
			    throw new ProtocolException("Payload length is negative or zero.");
    		
		    /* Read payload. */
		    if(this.Receive(buffer, payloadLength) != payloadLength)
			    throw new ProtocolException("Failed to read payload.");
	    }

        public void SendPacket(Int32 command, ByteBuffer payload)
        {
		    ByteBuffer buffer = ByteBuffer.Allocate(1 + 2 + payload.Remaining);
    		
		    /* Set IV. */
		    this._session.ShannonSend.nonce(IntegerUtilities.ToBytes(this._session.KeySendIv));
    		
		    /* Build packet. */
		    buffer.Put((Byte)command);
		    buffer.PutShort((Int16)payload.Remaining);
		    buffer.Put(payload.ToArray());
    		
		    Byte[] bytes = buffer.ToArray();
		    Byte[] mac = new Byte[4];
    		
		    /* Encrypt packet and get MAC. */
		    this._session.ShannonSend.encrypt(bytes);
		    this._session.ShannonSend.finish(mac);
    		
		    buffer = ByteBuffer.Allocate(buffer.Position + 4);
		    buffer.Put(bytes);
		    buffer.Put(mac);
		    buffer.Flip();
    		
		    /* Send encrypted packet. */
		    this.Send(buffer);
    		
		    /* Increment IV. */
		    this._session.KeySendIv++;
	    }

        public void SendPacket(Int32 command)
        {
		    this.SendPacket(command, ByteBuffer.Allocate(0));
	    }

        public void ReceivePacket()
        {
		    Byte[] header = new Byte[3];
		    Int32 command, payloadLength, headerLength = 3, macLength = 4;
    		
		    /* Read header. */
		    if(this.Receive(header, headerLength) != headerLength)
			    throw new ProtocolException("Failed to read header.");
            
    		
		    /* Set IV. */
		    this._session.ShannonRecv.nonce(IntegerUtilities.ToBytes(this._session.KeyRecvIv));
    		
		    /* Decrypt header. */
		    this._session.ShannonRecv.decrypt(header);
    		
		    /* Get command and payload length from header. */
		    ByteBuffer headerBuffer = ByteBuffer.Wrap(header);
    		
		    command = headerBuffer.Get() & 0xff;
		    payloadLength = headerBuffer.GetShort() & 0xffff; //FIXME: Wrong value, expected: 4. Check decryption
    		
		    /* Allocate buffer. Account for MAC. */
		    Byte[] bytes = new Byte[payloadLength + macLength];
		    //ByteBuffer buffer = ByteBuffer.Wrap(bytes);
    		
		    /* Limit buffer to payload length, so we can read the payload. */
		    //buffer.Limit(payloadLength);

		    try
            {
                //for (int n = payloadLength, r; n > 0 && (r = this._channel.Read(buffer)) > 0; n -= r);

                Int32 offset = 0, read = -1;
                while (offset < payloadLength && read != 0)
                {
                    //read = this._channel.GetStream().Read(bytes, offset, bytes.Length - offset);
                    read = this._channel.Client.Receive(bytes, offset, payloadLength - offset, SocketFlags.None);
                    offset += read;
                }
		    }
		    catch (Exception ex)
            {
			    throw new ProtocolException("Failed to read payload.", ex);
            }
    		
		    /* Extend it again to payload and mac length. */
		    //buffer.Limit(payloadLength + macLength);
    		
		    try
            {
			    //for(int n = macLength, r; n > 0 && (r = this.channel.read(buffer)) > 0; n -= r);

                Int32 offset = payloadLength, read = -1;
                while (offset < bytes.Length && read != 0)
                {
                    //read = this._channel.GetStream().Read(bytes, offset, bytes.Length - offset);
                    read = this._channel.Client.Receive(bytes, offset, bytes.Length - offset, SocketFlags.None);
                    offset += read;
                }
		    }
		    catch (Exception ex)
            {
			    throw new ProtocolException("Failed to read MAC.", ex);
		    }
    		
		    /* Decrypt payload. */
		    this._session.ShannonRecv.decrypt(bytes);
    		
		    /* Get payload bytes from buffer (throw away MAC). */
		    Byte[] payload = new Byte[payloadLength];
    		
		    //buffer.Flip();
            ByteBuffer buffer = ByteBuffer.Wrap(bytes);
		    buffer.Get(payload);
    		
		    /* Increment IV. */
		    this._session.KeyRecvIv++;
    		
		    /* Fire events. */
		    foreach (ICommandListener listener in this._listeners)
            {
			    listener.CommandReceived(command, payload);
		    }
	    }

        public void SendCacheHash() 
        {
		    ByteBuffer buffer = ByteBuffer.Allocate(20);
    		
		    buffer.Put(this._session.CacheHash);
		    buffer.Flip();
    		
		    this.SendPacket((Int32)Command.COMMAND_CACHEHASH, buffer);
	    }

        /* Request ads. The response is GZIP compressed XML. */
        public void SendAdRequest(IChannelListener listener, Int32 type)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("Ad-Channel", ChannelType.TYPE_AD, listener);
		    ByteBuffer buffer  = ByteBuffer.Allocate(2 + 1);
    		
		    /* Append channel id and ad type. */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put((Byte)type);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_REQUESTAD, buffer);
	    }

        /* Get a toplist. The response comes as GZIP compressed XML. */
        public void SendToplistRequest(IChannelListener listener, Dictionary<String, String> paramsarg)
        {
		    /* Check if type parameter is present. */
		    if (!paramsarg.ContainsKey("type"))
			    throw new ArgumentException("Parameter 'type' not given!");
    		
		    /* Create a map of parameters and calculate their length. */
		    Dictionary<Byte[], Byte[]> parameters = new Dictionary<Byte[], Byte[]>();
		    Int32 parametersLength = 0;
    		
		    foreach (var param in paramsarg)
            {
			    if(param.Key == null || param.Value == null)
				    continue;
    			
			    Byte[] key   = Encoding.UTF8.GetBytes(param.Key);
			    Byte[] value = Encoding.UTF8.GetBytes(param.Value);
    			
			    parametersLength += 1 + 2 + key.Length + value.Length;
    			
			    parameters.Add(key, value);
		    }
    		
		    /* Create channel and buffer. */
		    Channel.Channel channel = new Channel.Channel("Toplist-Channel", ChannelType.TYPE_TOPLIST, listener);
		    ByteBuffer buffer  = ByteBuffer.Allocate(2 + 2 + 2 + parametersLength);
    		
		    /* Append channel id, some values, query length and query. */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.PutShort((Int16)0x0000);
		    buffer.PutShort((Int16)0x0000);
    		
		    foreach (var parameter in parameters)
            {
			    Byte[] key = parameter.Key;
			    Byte[] value = parameter.Value;
    			
			    buffer.Put((Byte)key.Length);
			    buffer.PutShort((Int16)value.Length);
			    buffer.Put(key);
			    buffer.Put(value);
		    }
    		
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_GETTOPLIST, buffer);
	    }

        /* Request image using a 20 byte id. The response is a JPG. */
	    public void SendImageRequest(IChannelListener listener, String id)
        {
		    /* Create channel and buffer. */
		    Channel.Channel channel = new Channel.Channel("Image-Channel", ChannelType.TYPE_IMAGE, listener);
		    ByteBuffer buffer  = ByteBuffer.Allocate(2 + 20);
    		
		    /* Check length of id. */
		    if(id.Length != 40)
			    throw new ArgumentException("Image id needs to have a length of 40.");
    		
		    /* Append channel id and image hash. */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put(Hex.ToBytes(id));
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_IMAGE, buffer);
	    }

        /* Search music. The response comes as GZIP compressed XML. */
	    public void SendSearchQuery(IChannelListener listener, String query, Int32 offset, Int32 limit)
        {
		    /* Create channel and buffer. */
            Byte[] queryBytes = Encoding.UTF8.GetBytes(query);
            Channel.Channel channel = new Channel.Channel("Search-Channel", ChannelType.TYPE_SEARCH, listener);
		    ByteBuffer buffer = ByteBuffer.Allocate(2 + 4 + 4 + 2 + 1 + queryBytes.Length);
    		
		    /* Check offset and limit. */
		    if (offset < 0)
			    throw new ArgumentException("Offset needs to be >= 0");
		    else if ((limit < 0 && limit != -1) || limit == 0)
			    throw new ArgumentException("Limit needs to be either -1 for no limit or > 0");
    		
		    /* Append channel id, some values, query length and query. */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.PutInt(offset); /* Result offset. */
		    buffer.PutInt(limit); /* Reply limit. */
		    buffer.PutShort((Int16)0x0000);
		    buffer.Put((Byte)queryBytes.Length);
		    buffer.Put(queryBytes);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_SEARCH, buffer);
	    }

        /* Search music. The response comes as GZIP compressed XML. */
	    public void SendSearchQuery(IChannelListener listener, String query)
        {
		    this.SendSearchQuery(listener, query, 0, -1);
	    }
    	
	    /* Notify server we're going to play. */
	    public void SendTokenNotify()
        {
		    this.SendPacket((Int32)Command.COMMAND_TOKENNOTIFY);
	    }

        /* Request AES key for a track. */
	    public void SendAesKeyRequest(IChannelListener listener, Track track, Media.File file)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("AES-Key-Channel", ChannelType.TYPE_AESKEY, listener);
		    ByteBuffer buffer  = ByteBuffer.Allocate(20 + 16 + 2 + 2);
    		
		    /* Request the AES key for this file by sending the file id and track id. */
		    buffer.Put(Hex.ToBytes(file.Id)); /* 20 bytes */
		    buffer.Put(Hex.ToBytes(track.Id)); /* 16 bytes */
		    buffer.PutShort((Int16)0x0000);
            buffer.PutShort((Int16)channel.Id);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_REQKEY, buffer);
	    }

        /* A demo wrapper for playing a track. */
	    public void SendPlayRequest(IChannelListener listener, Track track, Media.File file)
        {
		    /* 
		     * Notify the server about our intention to play music, there by allowing
		     * it to request other players on the same account to pause.
		     * 
		     * Yet another client side restriction to annony those who share their
		     * Spotify account with not yet invited friends. And as a bonus it won't
		     * play commercials and waste bandwidth in vain.
		     */
		    this.SendPacket((Int32)Command.COMMAND_REQUESTPLAY);
		    this.SendAesKeyRequest(listener, track, file);
	    }

        /*
	     * Request a part of the encrypted file from the server.
	     * 
	     * The data should be decrypted using AES key in CTR mode
	     * with AES key provided and a static IV, incremented for
	     * each 16 byte data processed.
	     */
	    public void SendSubstreamRequest(IChannelListener listener, Media.File file, Int32 offset, Int32 length)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("Substream-Channel", ChannelType.TYPE_SUBSTREAM, listener);
		    ByteBuffer buffer = ByteBuffer.Allocate(2 + 2 + 2 + 2 + 2 + 2 + 4 + 20 + 4 + 4);
    		
		    /* Append channel id. */
		    buffer.PutShort((Int16)channel.Id);
    		
		    /* Unknown 10 bytes. */
            buffer.PutShort((Int16)0x0800);
            buffer.PutShort((Int16)0x0000);
            buffer.PutShort((Int16)0x0000);
            buffer.PutShort((Int16)0x0000);
            buffer.PutShort((Int16)0x4e20);
    		
		    /* Unknown (static value) */
		    buffer.PutInt(200 * 1000);
    		
		    /* 20 bytes file id. */
		    buffer.Put(Hex.ToBytes(file.Id));
    		
		    if(offset % 4096 != 0 || length % 4096 != 0)
			    throw new ArgumentException("Offset and length need to be a multiple of 4096.");
    		
		    offset >>= 2;
		    length >>= 2;
    		
		    /* Append offset and length. */
		    buffer.PutInt(offset);
		    buffer.PutInt(offset + length);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_GETSUBSTREAM, buffer);
	    }

        /*
	     * Get metadata for an artist (type = 1), album (type = 2) or a
	     * list of tracks (type = 3). The response comes as compressed XML.
	     */
	    public void SendBrowseRequest(IChannelListener listener, int type, IList<String> ids)
        {
		    /* Create channel and buffer. */
		    Channel.Channel channel = new Channel.Channel("Browse-Channel", ChannelType.TYPE_BROWSE, listener);
		    ByteBuffer buffer  = ByteBuffer.Allocate(2 + 1 + ids.Count * 16 + ((type == 1 || type == 2)?4:0));
    		
		    /* Check arguments. */
		    if(type != 1 && type != 2 && type != 3)
			    throw new ArgumentException("Type needs to be 1, 2 or 3.");
		    else if((type == 1 && type == 2) && ids.Count != 1)
			    throw new ArgumentException("Types 1 and 2 only accept a single id.");
    		
		    /* Append channel id and type. */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put((Byte)type);
    		
		    /* Append (16 byte binary, 32 byte hex string) ids. */
		    foreach (String id in ids)
            {
			    /* Check length of id. */
			    if(id.Length != 32)
				    throw new ArgumentException("Id needs to have a length of 32.");
    			
			    buffer.Put(Hex.ToBytes(id));
		    }
    		
		    /* Append zero. */
		    if(type == 1 || type == 2)
			    buffer.PutInt(0);
    		
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_BROWSE, buffer);
	    }

        /* Browse with only one id. */
	    public void SendBrowseRequest(IChannelListener listener, Int32 type, String id)
        {
            List<String> list = new List<String>();
    		
		    list.Add(id);
    		
		    this.SendBrowseRequest(listener, type, list);
	    }

        /* Request user playlists. The response comes as plain XML. */
	    public void SendUserPlaylistsRequest(IChannelListener listener)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("Playlists-Channel", ChannelType.TYPE_PLAYLIST, listener);
		    ByteBuffer buffer  = ByteBuffer.Allocate(2 + 16 + 1 + 4 + 4 + 4 + 1);
    		
		    /* Append channel id, playlist id and some bytes... */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put(Hex.ToBytes("00000000000000000000000000000000")); /* 16 bytes */
		    buffer.Put((Byte)0x00); /* Playlists identifier. */
		    buffer.PutInt(-1); /* Playlist history. -1: current. 0: changes since version 0, 1: since version 1, etc. */
		    buffer.PutInt(0);
		    buffer.PutInt(-1);
		    buffer.Put((byte)0x01);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_GETPLAYLIST, buffer);
	    }

        /* Change playlists. The response comes as plain XML. */
	    public void SendChangeUserPlaylists(IChannelListener listener, PlaylistContainer playlists, String xml)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("Change-Playlists-Channel", ChannelType.TYPE_PLAYLIST, listener);
            Byte[] xmlBytes = Encoding.ASCII.GetBytes(xml); //FIXME: Correct Encoding?
            ByteBuffer buffer = ByteBuffer.Allocate(2 + 16 + 1 + 4 + 4 + 4 + 1 + 1 + xmlBytes.Length);
    		
		    /* Append channel id, playlist id and some bytes... */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put(Hex.ToBytes("00000000000000000000000000000000")); /* 16 bytes */
		    buffer.Put((Byte)0x00); /* Playlists identifier. */
		    buffer.PutInt((Int32)playlists.Revision);
		    buffer.PutInt(playlists.Playlists.Count);
		    buffer.PutInt((Int32)playlists.GetChecksum());
		    buffer.Put((Byte)0x00); /* Collaborative */
		    buffer.Put((Byte)0x03); /* Unknown */
		    buffer.Put(xmlBytes);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_CHANGEPLAYLIST, buffer);
	    }

        /* Request playlist details. The response comes as plain XML. */
	    public void SendPlaylistRequest(IChannelListener listener, String id)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("Playlist-Channel", ChannelType.TYPE_PLAYLIST, listener);
		    ByteBuffer buffer  = ByteBuffer.Allocate(2 + 16 + 1 + 4 + 4 + 4 + 1);
    		
		    /* Check length of id. */
		    if(id.Length != 32)
			    throw new ArgumentException("Playlist id needs to have a length of 32.");
    		
		    /* Append channel id, playlist id and some bytes... */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put(Hex.ToBytes(id)); /* 16 bytes */
		    buffer.Put((Byte)0x02); /* Playlist identifier. */
		    buffer.PutInt(-1); /* Playlist history. -1: current. 0: last change. */
		    buffer.PutInt(0);
		    buffer.PutInt(-1);
		    buffer.Put((Byte)0x01);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_GETPLAYLIST, buffer);
	    }

        /* Change playlist. The response comes as plain XML. */
	    public void SendChangePlaylist(IChannelListener listener, Playlist playlist, String xml)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("Change-Playlist-Channel", ChannelType.TYPE_PLAYLIST, listener);
            Byte[] xmlBytes = Encoding.ASCII.GetBytes(xml); //FIXME: Correct Encoding?
            ByteBuffer buffer = ByteBuffer.Allocate(2 + 16 + 1 + 4 + 4 + 4 + 1 + 1 + xmlBytes.Length);
    		
		    /* Append channel id, playlist id and some bytes... */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put(Hex.ToBytes(playlist.Id)); /* 16 bytes */
		    buffer.Put((Byte)0x02); /* Playlist identifier. */
		    buffer.PutInt((Int32)playlist.Revision);
		    buffer.PutInt(playlist.Tracks.Count);
		    buffer.PutInt((Int32)playlist.GetChecksum());
		    buffer.Put((Byte)(playlist.Collaborative ? 0x01 : 0x00));
		    buffer.Put((Byte)0x03); /* Unknown */
            buffer.Put(xmlBytes);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_CHANGEPLAYLIST, buffer);
	    }

        /* Create playlist. The response comes as plain XML. */
	    public void SendCreatePlaylist(IChannelListener listener, Playlist playlist, String xml)
        {
		    /* Create channel and buffer. */
            Channel.Channel channel = new Channel.Channel("Change-Playlist-Channel", ChannelType.TYPE_PLAYLIST, listener);
            Byte[] xmlBytes = Encoding.ASCII.GetBytes(xml); //FIXME: Correct Encoding?
            ByteBuffer buffer = ByteBuffer.Allocate(2 + 16 + 1 + 4 + 4 + 4 + 1 + 1 + xmlBytes.Length);
    		
		    /* Append channel id, playlist id and some bytes... */
		    buffer.PutShort((Int16)channel.Id);
		    buffer.Put(Hex.ToBytes(playlist.Id)); /* 16 bytes */
		    buffer.Put((Byte)0x02); /* Playlist identifier. */
		    buffer.PutInt(0);
		    buffer.PutInt(0);
		    buffer.PutInt(-1); /* -1: Create playlist. */
		    buffer.Put((Byte)(playlist.Collaborative ? 0x01 : 0x00));
		    buffer.Put((Byte)0x03); /* Unknown */
		    buffer.Put(xmlBytes);
		    buffer.Flip();
    		
		    /* Register channel. */
		    Channel.Channel.Register(channel);
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_CHANGEPLAYLIST, buffer);
	    }

        /* Ping reply (pong). */
	    public void SendPong()
        {
		    ByteBuffer buffer = ByteBuffer.Allocate(4);
    		
		    /* TODO: Append timestamp? */
		    buffer.PutInt(0x00000000);
		    buffer.Flip();
    		
		    /* Send packet. */
		    this.SendPacket((Int32)Command.COMMAND_PONG, buffer);
	    }

        /* Send bytes. */
	    private void Send(ByteBuffer buffer)
        {
		    try
            {
#if ADAPTER_MODE //FIXME
                DebugAdapter.SendData(buffer.ToArray());
#else
			    this._channel.Client.Send(buffer.ToArray());
#endif
		    }
		    catch (Exception e)
            {
			    throw new ProtocolException("Error writing data to socket.", e);
		    }
	    }

        /* Receive a single byte. */
	    private Int32 Receive()
        {
    		Byte[] buffer = new Byte[1];
		    try
            {
#if ADAPTER_MODE //FIXME
                DebugAdapter.ReceiveData(buffer);
#else
                this._channel.Client.Receive(buffer);
#endif
    			
			    return buffer[0] & 0xff;
		    }
		    catch(Exception e)
            {
			    throw new ProtocolException("Error reading data from socket.", e);
		    }
	    }

        /* Receive bytes. */
	    private Int32 Receive(Byte[] buffer, Int32 len)
        {
		    return this.Receive(buffer, 0, len);
	    }

        /* Receive bytes. */
	    private Int32 Receive(Byte[] bytes, Int32 off, Int32 len)
        {
		    Int32 n = 0;
		    try
            {
                Int32 lastRead = -1;
                while (lastRead != 0 && n < len)
                {
#if ADAPTER_MODE //FIXME
                    lastRead = DebugAdapter.ReceiveData(bytes, off + n, len - n);
#else
                    lastRead = this._channel.Client.Receive(bytes, off + n, len - n, SocketFlags.None);
#endif
                    n += lastRead;
                }
		    }
		    catch (Exception e)
            {
			    throw new ProtocolException("Error reading data from socket.", e);
		    }
		    return n;
	    }

        #endregion


        #region construction

        public Protocol(Session session)
        {
            this._session = session;
        }

        #endregion
    }
}
