using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Math;

namespace SharpSpotLib.Util
{
    public static class BaseConvert
    {
        #region fields

        public const Int32 MIN_RADIX = 2;
        public const Int32 MAX_RADIX = 62;
        private static readonly String CHARACTERS = "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";

        #endregion

        #region methods

        public static Int32 Digit(Char ch, Int32 radix)
        {
            /* 
             * If the radix is less or equal to 36, we automatically convert upper-case
             * characters to lower-case. Otherwise upper-case characters won't be valid
             * in the specified radix, but we still want them to be valid (for example
             * when using hexadecimal numbers).
             */
            if (radix <= 36)
                ch = Char.ToLower(ch);

            /* The index of our character is the digit we want. */
            return CHARACTERS.Substring(0, radix).IndexOf(ch);
        }

        public static Char ForDigit(Int32 digit, Int32 radix)
        {
            /* The character at the index equal to the digit is the one we want. */
            return CHARACTERS.Substring(0, radix)[digit];
        }

        public static String Convert(String source, Int32 sourceRadix, Int32 targetRadix)
        {
            /* Check if radix arguments are within the allowed range. */
            if (sourceRadix < MIN_RADIX || sourceRadix > MAX_RADIX)
                throw new ArgumentOutOfRangeException("sourceRadix", "Source radix needs to be in a range from " + MIN_RADIX + " to " + MAX_RADIX);
            if (targetRadix < MIN_RADIX || targetRadix > MAX_RADIX)
                throw new ArgumentOutOfRangeException("targetRadix", "Target radix needs to be in a range from " + MIN_RADIX + " to " + MAX_RADIX);

            BigInteger radixFrom = new BigInteger((UInt32)sourceRadix);
            BigInteger value = new BigInteger(0);
            BigInteger multiplier = new BigInteger(1);

            for (Int32 i = source.Length - 1; i >= 0; i--)
            {
                Int32 digit = Digit(source[i], sourceRadix);
                if (digit == -1)
                    throw new ArgumentException("The character '" + source[i] + "' is not defined for the source radix.", "sourceRadix");
                value += multiplier * digit;
                multiplier = multiplier * radixFrom;
            }

            return value.ToString((UInt32)targetRadix, CHARACTERS.Substring(0, targetRadix));
        }

        #endregion
    }
}
