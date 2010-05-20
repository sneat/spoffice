using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Crypto
{
    internal class DHPublicKey
    {
        private Byte[] _key;

        /*public Byte[] P
        {
            get;
            private set;
        }

        public Byte[] G
        {
            get;
            private set;
        }*/

        public Byte[] KeyExchangeData
        {
            get
            {
                return _key;
            }
        }

        public Byte[] ToByteArray()
        {
            return KeyExchangeData;
        }

        /*
        public DHPublicKey(Byte[] p, Byte[]g, Byte[] keyExcahngeData)
        {
            P = p;
            G = g;
            _key = keyExcahngeData;
        }*/

        public DHPublicKey(Byte[] keyExcahngeData)
        {
            _key = keyExcahngeData;
        }

    }
}
