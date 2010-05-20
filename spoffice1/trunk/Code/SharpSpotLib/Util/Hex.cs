using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace SharpSpotLib.Util
{
    internal static class Hex
    {
        public static byte[] ToBytes(String hex)
        {
            if (!IsHex(hex))
                throw new ArgumentException("Input string is not a valid hexadecimal string.");
            Byte[] bytes = new Byte[hex.Length / 2];
            for (int i = 0; i < hex.Length - 1; i += 2)
            {
                bytes[i / 2] = Byte.Parse(new String(new Char[] { hex[i], hex[i + 1] }), 
                    System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;
        }

        public static String ToHex(Byte[] bytes)
        {
            String hex = "";
            foreach (Byte b in bytes)
                hex += b.ToString("X2");
            return hex;
        }

        public static Boolean IsHex(String hex)
        {
            return (hex.Length % 2 == 0) && Regex.IsMatch(hex, @"[0-9A-Fa-f]+");
        }
    }
}
