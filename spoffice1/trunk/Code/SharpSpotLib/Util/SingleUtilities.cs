using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace SharpSpotLib.Util
{
    internal static class SingleUtilities
    {
        public static Boolean TryParse(String s, out Single result)
        {
            result = 0f;
            if (String.IsNullOrEmpty(s))
                return false;
            try
            {
                result = Single.Parse(s);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static Single IntBitsToFloat(Int32 value)
        {
            return BitConverter.ToSingle(BitConverter.GetBytes(value), 0);
        }
    }
}
