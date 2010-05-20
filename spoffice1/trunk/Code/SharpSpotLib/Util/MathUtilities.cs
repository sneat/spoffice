using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Util
{
    internal static class MathUtilities
    {
        public static Single Map(Single value, Single imin, Single imax, Single omin, Single omax)
        {
            return omin + (omax - omin) * ((value - imin) / (imax - imin));
        }

        public static Single Constrain(Single value, Single min, Single max)
        {
            return Math.Max(Math.Min(value, max), min);
        }
    }
}
