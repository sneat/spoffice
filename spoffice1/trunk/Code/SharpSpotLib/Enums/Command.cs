using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Enums
{
    internal enum Command
    {
        /* Core functionality. */
	    COMMAND_SECRETBLK    = 0x02,
	    COMMAND_PING         = 0x04,
	    COMMAND_GETSUBSTREAM = 0x08,
	    COMMAND_CHANNELDATA  = 0x09,
	    COMMAND_CHANNELERR   = 0x0a,
	    COMMAND_CHANNELABRT  = 0x0b,
	    COMMAND_REQKEY       = 0x0c,
	    COMMAND_AESKEY       = 0x0d,
	    COMMAND_CACHEHASH    = 0x0f,
	    COMMAND_SHAHASH      = 0x10,
	    COMMAND_IMAGE        = 0x19,
	    COMMAND_TOKENNOTIFY  = 0x4f,
    	
	    /* Rights management. */
	    COMMAND_COUNTRYCODE = 0x1b,
    	
	    /* P2P related. */
	    COMMAND_P2P_SETUP   = 0x20,
	    COMMAND_P2P_INITBLK = 0x21,
    	
	    /* Search and metadata. */
	    COMMAND_BROWSE         = 0x30,
	    COMMAND_SEARCH         = 0x31,
	    COMMAND_GETPLAYLIST    = 0x35,
	    COMMAND_CHANGEPLAYLIST = 0x36,
	    COMMAND_GETTOPLIST     = 0x38,
    	
	    /* Session management. */
	    COMMAND_NOTIFY      = 0x42,
	    COMMAND_LOG         = 0x48,
	    COMMAND_PONG        = 0x49,
	    COMMAND_PONGACK     = 0x4a,
	    COMMAND_PAUSE       = 0x4b,
	    COMMAND_REQUESTAD   = 0x4e,
	    COMMAND_REQUESTPLAY = 0x4f,
    	
	    /* Internal. */
	    COMMAND_PRODINFO    = 0x50,
	    COMMAND_WELCOME     = 0x69,
    }
}
