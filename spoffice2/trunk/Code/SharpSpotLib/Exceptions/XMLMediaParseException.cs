using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Exceptions
{
    public class XMLMediaParseException : Exception
    {
        public XMLMediaParseException(String message)
            : base(message)
        {
        }

        public XMLMediaParseException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
