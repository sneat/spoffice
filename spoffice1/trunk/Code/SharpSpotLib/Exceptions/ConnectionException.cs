using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Exceptions
{
    public class ConnectionException : Exception
    {
        public ConnectionException(String message)
            : base(message)
        {
        }

        public ConnectionException(String message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
