using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Util
{
    internal static class ShortUtilities
    {
        public static Byte[] ToBytes(Int16 i)
        {
            Byte[] b = new Byte[2];
            b[0] = (Byte)(i >> 8);
            b[1] = (Byte)(i);
            return b;
        }

        public static UInt16 BytesToUnsignedShort(Byte[] b, Int32 offset)
        {
		    /* Check length of byte array. */
		    if(b.Length < offset + 2)
			    throw new ArgumentException("Not enough bytes in array.");
		
		    /* Convert and return value. */
		    return (UInt16)(((b[offset] << 8) & 0xFFFF) | ((b[offset + 1]) & 0x00FF));
        }

        public static short BytesToShort(Byte[] b, Int32 offset)
        {
            if (b.Length < offset + 2)
                throw new ArgumentException("Not enough bytes in array.");

            /* Convert and return value. */
            return (Int16)(((b[offset] << 8) & 0xFFFF) | (b[offset + 1]) & 0x00FF);
        }

        /* WRONG ENDIAN:
        public static Byte[] ToBytes(Int16 i)
        {
            return BitConverter.GetBytes(i);
        }

        public static UInt16 BytesToUnsignedShort(Byte[] b, Int32 offset)
        {
            return BitConverter.ToUInt16(b, offset);
        }

        public static short BytesToShort(Byte[] b, Int32 offset)
        {
            return BitConverter.ToInt16(b, offset);
        }*/

        public static UInt16 BytesToUnsignedShort(Byte[] b)
        {
            return BytesToUnsignedShort(b, 0);
        }

        public static Int16 BytesToShort(Byte[] b)
        {
            return BytesToShort(b, 0);
        }
    }
}
