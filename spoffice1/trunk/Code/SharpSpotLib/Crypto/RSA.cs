using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SharpSpotLib.Crypto
{
    internal class RSA
    {
        #region fields

        #endregion

        #region methods

        public static RSAKeyPair GenerateKeyPair(Int32 keysize)
        {
            var item = new RSACryptoServiceProvider(keysize);
            return new RSAKeyPair(
                new RSAPrivateKey(item.ExportParameters(true).D),
                new RSAPublicKey(item.ExportParameters(false).Modulus));
        }

        #endregion
    }
}
