using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Exceptions
{
    public class ProtocolException : Exception
    {
        public ProtocolException(String message)
            : base(message)
        {
        }

        public ProtocolException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
