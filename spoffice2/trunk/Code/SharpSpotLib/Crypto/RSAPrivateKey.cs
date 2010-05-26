using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Crypto
{
    internal class RSAPrivateKey
    {
        private Byte[] _key;

        public Byte[] ToByteArray()
        {
            return _key;
        }

        public RSAPrivateKey(Byte[] key)
        {
            _key = key;
        }
    }
}
