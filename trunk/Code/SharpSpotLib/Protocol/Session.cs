//FIXME: Debug
//#define ADAPTER_MODE

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using SharpSpotLib.Crypto;
using SharpSpotLib.Exceptions;
using SharpSpotLib.Util;

namespace SharpSpotLib.Protocol
{
    internal class Session
    {
        #region constants
        
	    /* Always up to date! ;-P */
	    public const Int32 CLIENT_REVISION = 65324;

        #endregion


        #region fields

        private Protocol _protocol;

        /* Client identification */
        /* Set client identification (Spotify 0.3.12 / r45126). */
        private Byte[] _clientId = new Byte[] { 0x01, 0x04, 0x01, 0x01 };
        private Int32 _clientRevision = CLIENT_REVISION;

        /* 16 bytes of Shannon encryption output with random key */
        private Byte[] _clientRandom;
        private Byte[] _serverRandom = new Byte[16];

        /* 
	     * Blob (1536-bit RSA signature at offset 128)
	     * is received at offset 16 in the cmd=0x02 packet.
	     */
        private Byte[] _serverBlob = new Byte[256];
    	
	    /* Username, password, salt, auth hash, auth HMAC and country. */
        private Byte[] _username;
        private Byte[] _password;
        private Byte[] _salt = new Byte[10];
        private Byte[] _authHash = new Byte[20];
        //private String _country;
    	
	    /* DH and RSA keys. */
        private DHKeyPair _dhClientKeyPair;
        private DHPublicKey _dhServerPublicKey;
        private Byte[] _dhSharedKey = new Byte[96];
        private RSAKeyPair _rsaClientKeyPair;
	
	    /* 
	     * Output form HMAC SHA-1, used for keying HMAC
	     * and for keying Shannon stream cipher.
	     */
        private Byte[] _keyHmac = new Byte[20];
        private Byte[] _authHmac = new Byte[20];
        private Byte[] _keyRecv = new Byte[32];
        private Byte[] _keySend = new Byte[32];
        private Int32 _keyRecvIv = 0;
        private Int32 _keySendIv = 0;
    	
	    /* Shannon stream cipher */
        private Shannon _shannonSend = new Shannon();
        private Shannon _shannonRecv = new Shannon();
	
	    /*
	     * Waste some CPU time while computing a 32-bit value,
	     * that byteswapped and XOR'ed with a magic, modulus
	     * 2^deniminator becomes zero.
	     */
        private Int32 _puzzleDenominator = 0;
        private Int32 _puzzleMagic = 0;
        private Byte[] _puzzleSolution = new Byte[8];
    	
	    /* Cache hash. Automatically generated, but we're lazy. */
        private Byte[] _cacheHash;
    	
	    /* Needed for auth hmac. */
        private Byte[] _initialClientPacket = null;
        private Byte[] _initialServerPacket = null;

        #endregion


        #region properties

        public String Username
        {
            get
            {
                return Encoding.ASCII.GetString(_username, 0, _username.Length);
            }
        }

        public Byte[] UsernameBytes
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        public RSAPublicKey RSAPublicKey
        {
            get
            {
                return this._rsaClientKeyPair.PublicKey;
            }
        }

        public DHPublicKey DHPublicKey
        {
            get
            {
                return this._dhClientKeyPair.PublicKey;
            }
        }

        public Byte[] ClientRandom
        {
            get { return _clientRandom; }
        }

        public Byte[] ServerRandom
        {
            get { return _serverRandom; }
            set { _serverRandom = value; }
        }

        public Byte[] ServerBlob
        {
            get { return _serverBlob; }
            set { _serverBlob = value; }
        }

        public Byte[] Salt
        {
            get { return _salt; }
            set { _salt = value; }
        }

        public Byte[] ClientId
        {
            get { return _clientId; }
        }

        public Int32 ClientRevision
        {
            get { return _clientRevision; }
        }

        public Byte[] InitialClientPacket
        {
            get { return _initialClientPacket; }
            set { _initialClientPacket = value; }
        }

        public Byte[] InitialServerPacket
        {
            get { return _initialServerPacket; }
            set { _initialServerPacket = value; }
        }

        public DHPublicKey DHServerPublicKey
        {
            get { return _dhServerPublicKey; }
            set { _dhServerPublicKey = value; }
        }

        public Int32 PuzzleDenominator
        {
            get { return _puzzleDenominator; }
            set { _puzzleDenominator = value; ; }
        }

        public Int32 PuzzleMagic
        {
            get { return _puzzleMagic; }
            set { _puzzleMagic = value; ; }
        }

        public Byte[] PuzzleSolution
        {
            get { return _puzzleSolution; }
            //set { _puzzleSolution = value; ; }
        }

        public Byte[] AuthHmac
        {
            get { return _authHmac; }
            //set { _authHmac = value; }
        }

        public Shannon ShannonSend
        {
            get { return _shannonSend; }
        }

        public Shannon ShannonRecv
        {
            get { return _shannonRecv; }
        }

        public Int32 KeySendIv
        {
            get { return _keySendIv; }
            set { _keySendIv = value; }
        }

        public Int32 KeyRecvIv
        {
            get { return _keyRecvIv; }
            set { _keyRecvIv = value; }
        }

        public Byte[] CacheHash
        {
            get { return _cacheHash; }
        }
        

        #endregion


        #region methods

        public Protocol Authenticate(String username, String password)
        {
		    /* Set username and password. */
            
		    this._username = Encoding.ASCII.GetBytes(username);
		    this._password = Encoding.ASCII.GetBytes(password);
    		
		    /* Connect to a spotify server. */
		    this._protocol.Connect();
    		
		    /* Send and receive inital packets. */
		    try
            {
			    this._protocol.SendInitialPacket();
			    this._protocol.ReceiveInitialPacket();
		    }
		    catch (ProtocolException e)
            {
			    throw new AuthenticationException(e);
		    }
    		
		    /* Generate auth hash. */
		    this.GenerateAuthHash();
    		
		    /* Compute shared key (Diffie Hellman key exchange). */
		    this._dhSharedKey = DH.ComputeSharedKey(
			    this._dhClientKeyPair.PrivateKey,
			    this._dhServerPublicKey
		    );
    		
		    /* Prepare a message to authenticate. */
		    ByteBuffer buffer = ByteBuffer.Allocate(
			    this._authHash.Length + this._clientRandom.Length + this._serverRandom.Length + 1
		    );
    		
		    /* Append auth hash, client and server random to message. */
		    buffer.Put(this._authHash);
		    buffer.Put(this._clientRandom);
		    buffer.Put(this._serverRandom);
		    buffer.Put((Byte)0x00); /* Changed later */
		    buffer.Flip();
    		
		    /* Get message bytes and allocate space for HMACs. */
		    Byte[] bytes  = new Byte[buffer.Remaining];
		    Byte[] hmac   = new Byte[5 * 20];
		    Int32 offset = 0;
    		
		    buffer.Get(bytes);
    		
		    /* Run HMAC SHA-1 over message. 5 times. */
		    for(int i = 1; i <= 5; i++){
			    /* Change last byte (53) of message. */
			    bytes[bytes.Length - 1] = (byte)i;
    			
			    /* Compute HMAC SHA-1 using the shared key. */
			    Hash.HmacSha1(bytes, this._dhSharedKey, hmac, offset);
    			
			    /* Overwrite first 20 bytes of message with output from this round. */
			    for(int j = 0; j < 20; j++){
				    bytes[j] = hmac[offset + j];
			    }
    			
			    /* Advance to next position. */
			    offset += 20;
		    }
    		
		    /* Use field of HMACs to setup keys for Shannon stream cipher (key length: 32). */
		    //this._keySend = Arrays.copyOfRange(hmac, 20, 20 + 32);
            Array.Copy(hmac, 20, _keySend, 0, 32);
		    //this._keyRecv = Arrays.copyOfRange(hmac, 52, 52 + 32);
            Array.Copy(hmac, 52, _keyRecv, 0, 32);
    		
		    /* Set stream cipher keys. */
		    this._shannonSend.key(this._keySend);
		    this._shannonRecv.key(this._keyRecv);
    		
		    /* 
		     * First 20 bytes of HMAC output is used to key another HMAC computed
		     * for the second authentication packet send by the client.
		     */
		    //this._keyHmac = Arrays.copyOfRange(hmac, 0, 20);
            Array.Copy(hmac, 0, _keyHmac, 0, 20);
    		
		    /* Solve puzzle */
		    this.SolvePuzzle();
    		
		    /* Generate HMAC */
		    this.GenerateAuthHmac();
    		
		    /* Send authentication. */
		    try
            {
			    this._protocol.SendAuthenticationPacket();
			    this._protocol.ReceiveAuthenticationPacket();
		    }
		    catch(ProtocolException e)
            {
			    throw new AuthenticationException(e);
		    }
    		
		    return this._protocol;
	    }

        private void GenerateAuthHash()
        {
            Byte[] buffer = new Byte[this._salt.Length + 1 + this._password.Length];

            Array.Copy(this._salt, buffer, this._salt.Length);
            buffer[this._salt.Length] = (Byte)' ';
            Array.Copy(this._password, 0, buffer, this._salt.Length + 1, this._password.Length);

            //this._authHash = new SHA1CryptoServiceProvider().ComputeHash(buffer);
            this._authHash = Hash.Sha1(buffer);
        }

        private void GenerateAuthHmac()
        {
            ByteBuffer buffer = ByteBuffer.Allocate(
                this._initialClientPacket.Length +
                this._initialServerPacket.Length +
                1 + 1 + 2 + 4 + 0 + this._puzzleSolution.Length
            );

            buffer.Put(this._initialClientPacket);
            buffer.Put(this._initialServerPacket);
            buffer.Put((Byte)0); /* Random data length */
            buffer.Put((Byte)0); /* Unknown */
            buffer.PutShort((Int16)this._puzzleSolution.Length);
            buffer.PutInt(0x0000000); /* Unknown */
            buffer.Put(this._puzzleSolution); /* 8 bytes */

            this._authHmac = Hash.HmacSha1(buffer.ToArray(), this._keyHmac);
        }

        private void SolvePuzzle()
        {
            Int64 denominator;
            Int64 nominatorFromHash;
            Byte[] digest;

            Byte[] buffer = new Byte[this._serverRandom.Length + this._puzzleSolution.Length];

            /* Modulus operation by a power of two. */
            denominator = 1 << this._puzzleDenominator;
            denominator--;

            /* 
             * Compute a hash over random data until
             * (last dword byteswapped XOR magic number)
             * mod denominator by server produces zero.
             */
            do
            {
                /* Let's waste some precious pseudorandomness. */
                this._puzzleSolution = RandomBytes.GetRandomBytes(8);

                /* Buffer with server random and random bytes (puzzle solution). */
                Array.Copy(this._serverRandom, buffer, this._serverRandom.Length);
                Array.Copy(this._puzzleSolution, 0, buffer, this._serverRandom.Length, this._puzzleSolution.Length);
                
                /* Calculate digest. */
                digest = new SHA1CryptoServiceProvider().ComputeHash(buffer);

                /* Convert bytes to integer (Java is big-endian). */
                nominatorFromHash = ((digest[16] & 0xFF) << 24) |
                                    ((digest[17] & 0xFF) << 16) |
                                    ((digest[18] & 0xFF) << 8) |
                                    ((digest[19] & 0xFF));

                /* XOR with a fancy magic. */
                nominatorFromHash ^= this._puzzleMagic;
            } while ((nominatorFromHash & denominator) != 0);
        }

        #endregion


        #region construction

        /* Constructor for a new spotify session. */
        public Session()
        {
            this._protocol = new Protocol(this);

#if ADAPTER_MODE //FIXME
            this._clientRandom = DebugAdapter.SetClientRandom();
#else
            this._clientRandom = RandomBytes.GetRandomBytes(16);
#endif

            /*
             * Create a private and public DH key and allocate buffer
             * for shared key. This, along with key signing, is used
             * to securely agree on a session key for the Shannon stream
             * cipher.
             */
#if ADAPTER_MODE //FIXME
            this._dhClientKeyPair = DebugAdapter.SetDHClientKeyPair();
#else
            this._dhClientKeyPair = DH.GenerateKeyPair(768);
#endif

            /* Generate RSA key pair. */
#if ADAPTER_MODE //FIXME
            this._rsaClientKeyPair = DebugAdapter.SetRSAClientKeyPair();
#else
            this._rsaClientKeyPair = Crypto.RSA.GenerateKeyPair(1024);
#endif

            /* Found in Storage.dat (cache) at offset 16. Modify first byte of cache hash. */
            this._cacheHash = new Byte[]{
			    (Byte)0xf4, (Byte)0xc2, (Byte)0xaa, (Byte)0x05,
			    (Byte)0xe8, (Byte)0x25, (Byte)0xa7, (Byte)0xb5,
			    (Byte)0xe4, (Byte)0xe6, (Byte)0x59, (Byte)0x0f,
			    (Byte)0x3d, (Byte)0xd0, (Byte)0xbe, (Byte)0x0a,
			    (Byte)0xef, (Byte)0x20, (Byte)0x51, (Byte)0x95
		    };
            this._cacheHash[0] = (Byte)new Random().Next(255);
        }

        #endregion
    }
}
