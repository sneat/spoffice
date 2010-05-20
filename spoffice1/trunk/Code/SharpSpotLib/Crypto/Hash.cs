using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace SharpSpotLib.Crypto
{
    internal static class Hash
    {
        public static Byte[] Sha1(Byte[] buffer)
        {
            return new SHA1CryptoServiceProvider().ComputeHash(buffer);
        }

        public static Byte[] Md5(Byte[] buffer)
        {
            return new MD5CryptoServiceProvider().ComputeHash(buffer);
        }

        public static Byte[] HmacSha1(Byte[] buffer, Byte[] key)
        {
            
            return new HMACSHA1(key).ComputeHash(buffer);
        }

        public static void HmacSha1(Byte[] buffer, Byte[] key, Byte[] output, Int32 offset) 
        {
            Byte[] hash = HmacSha1(buffer, key);
            Array.Copy(hash, 0, output, offset, hash.Length);
        }
    }
}
