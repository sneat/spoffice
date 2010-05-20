using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Org.Mentalis.Security.Cryptography;

namespace SharpSpotLib.Crypto
{
    internal class DHPrivateKey
    {
        private DHParameters _key;

        public Byte[] P { get { return _key.P; } }
        public Byte[] G { get { return _key.G; } }
        public Byte[] X { get { return _key.X; } }

        public DHPrivateKey(DHParameters dhPrivateParams)
        {
            _key = dhPrivateParams;
        }
    }
}
