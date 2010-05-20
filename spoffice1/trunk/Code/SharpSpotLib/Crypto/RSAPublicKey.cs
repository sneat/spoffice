using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SharpSpotLib.Crypto
{
    internal class RSAPublicKey
    {
        private Byte[] _key;

        public Byte[] ToByteArray()
        {
            return _key;
        }

        public RSAPublicKey(Byte[] key)
        {
            _key = key;
        }
    }
}
