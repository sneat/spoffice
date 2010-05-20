using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Util
{
    internal static class IntegerUtilities
    {
        public static Boolean TryParse(String s, out Int32 result)
        {
            result = 0;
            if (String.IsNullOrEmpty(s))
                return false;
            try
            {
                result = Int32.Parse(s);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Byte[] ToBytes(Int32 i)
        {
            Byte[] b = new Byte[4];
            b[0] = (Byte)(i >> 24);
            b[1] = (Byte)(i >> 16);
            b[2] = (Byte)(i >> 8);
            b[3] = (Byte)(i);
            return b;
        }

        public static UInt32 BytesToUnsignedInteger(Byte[] b, Int32 offset)
        {
            //FIXME: Ugly
            Byte[] buffer = new Byte[4];
            Array.Copy(b, offset, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        public static Int32 BytesToInteger(Byte[] b, Int32 offset)
        {
            //FIXME: Ugly
            Byte[] buffer = new Byte[4];
            Array.Copy(b, offset, buffer, 0, buffer.Length);
            Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        /* WRONG ENDIAN
        public static Byte[] ToBytes(Int32 i)
        {
            return BitConverter.GetBytes(i);
        }

        public static UInt32 BytesToUnsignedInteger(Byte[] b, Int32 offset)
        {
            return BitConverter.ToUInt32(b, offset);
        }

        public static Int32 BytesToInteger(Byte[] b, Int32 offset)
        {
            return BitConverter.ToInt32(b, offset);
        }*/

        public static UInt32 BytesToUnsignedInteger(Byte[] b)
        {
            return BytesToUnsignedInteger(b, 0);
        }

        public static Int32 BytesToInteger(Byte[] b)
        {
            return BytesToInteger(b, 0);
        }
        
    }
}
