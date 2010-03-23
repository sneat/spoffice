using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class PlaylistContainer : IEnumerable<Playlist>
    {
        #region constants

        public static readonly PlaylistContainer EMPTY = new PlaylistContainer();

        #endregion


        #region fields

        private Int64 _checksum;

        #endregion


        #region properties

        public String Author { get; set; }
        public List<Playlist> Playlists { get; set; }
        public Int64 Revision { get; set; }

        #endregion


        #region methods

        public Int64 GetChecksum()
        {
            SpotifyChecksum checksum = new SpotifyChecksum();

            foreach (Playlist playlist in this.Playlists)
                checksum.Update(playlist);

            this._checksum = checksum.ChecksumValue;

            return this._checksum;
        }

        public void SetChecksum(Int64 checksum)
        {
            this._checksum = checksum;
        }

        public static PlaylistContainer FromXMLElement(XMLElement playlistsElement)
        {
		    PlaylistContainer playlists = new PlaylistContainer();
    		
		    /* Get "change" element. */
		    XMLElement changeElement = playlistsElement.GetChild("next-change").GetChild("change");
    		
		    /* Get author. */
		    playlists.Author = changeElement.GetChildText("user").Trim();
    		
		    /* Get items (comma separated list). */
		    if(changeElement.GetChild("ops").HasChild("add"))
            {
			    String items = changeElement.GetChild("ops").GetChild("add").GetChildText("items");
    			
			    foreach (String id in items.Split(new Char[] { ',' }))
                {
				    playlists.Playlists.Add(new Playlist(id.Trim().Substring(0, 32), "", playlists.Author, false));
			    }
		    }
    		
		    /* Get "version" element. */
		    XMLElement versionElement = playlistsElement.GetChild("next-change").GetChild("version");
    		
		    /* Split version string into parts. */
            String[] parts = versionElement.Text.Split(new Char[] { ',' });
    		
		    /* Set values. */
		    playlists.Revision = Int64.Parse(parts[0]);
            playlists.SetChecksum(Int64.Parse(parts[2]));
    		
		    return playlists;
	    }

        #endregion


        #region construction

        public PlaylistContainer()
        {
            this.Author = null;
            this.Playlists = new List<Playlist>();
            this.Revision = -1;
            this._checksum = -1;
        }

        #endregion


        #region IEnumerable<Playlist> Members

        public IEnumerator<Playlist> GetEnumerator()
        {
            return Playlists.GetEnumerator();
        }

        #endregion


        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return Playlists.GetEnumerator();
        }

        #endregion
    }
}
