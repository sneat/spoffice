using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Crypto
{
    internal static class RandomBytes
    {
        private static Random _rnd = new Random();

        public static void GetRandomBytes(ref Byte[] buffer)
        {
            _rnd.NextBytes(buffer);
        }

        public static Byte[] GetRandomBytes(Int32 length)
        {
            /* Create a buffer of the specified length. */
            Byte[] buffer = new Byte[length];
            GetRandomBytes(ref buffer);
            return buffer;
        }
    }
}
