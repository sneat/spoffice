using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Protocol
{
    internal class HostnamePortPair
    {
        private String _hostname = null;
        private Int32 _port = 0;

        public String Hostname { get { return _hostname; } }
        public Int32 Port { get { return _port; } }

        public HostnamePortPair(String hostname, Int32 port)
        {
            _hostname = hostname;
            _port = port;
        }

        public override string ToString()
        {
            return String.Format("{0}:{1}", Hostname, Port);
        }
    }
}
