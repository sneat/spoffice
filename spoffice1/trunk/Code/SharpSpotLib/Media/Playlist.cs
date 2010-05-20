using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class Playlist : IEnumerable<Track>
    {
        #region fields

        private Int64 _checksum; //FIXME: Never used?!

        #endregion


        #region properties

        public String Id { get; set; }
        public String Name { get; set; }
        public String Author { get; set; }
        public List<Track> Tracks { get; set; }
        public Int64 Revision { get; set; }
        public Boolean Collaborative { get; set; }

        public String URI
        {
            get
            {
                return String.Format("spotify:user:{0}:playlist:{1}", this.Author, SpotifyURI.ToBase62(this.Id));
            }
        }

        public String Link
        {
            get
            {
                return String.Format("http://open.spotify.com/user/{0}/playlist/{1}", this.Author, SpotifyURI.ToBase62(this.Id));
            }
        }

        #endregion


        #region methods

        public Int64 GetChecksum()
        {
		    SpotifyChecksum checksum = new SpotifyChecksum(); 
    		
		    foreach (Track track in this.Tracks)
			    checksum.Update(track);
    		
		    this._checksum = checksum.ChecksumValue;
    		
		    return this._checksum;
	    }

        public override Boolean Equals(Object obj)
        {
            if (obj is Playlist)
            {
                Playlist a = obj as Playlist;
                if (this.Id == a.Id)
                    return true;
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            return (this.Id != null) ? this.Id.GetHashCode() : 0;
        }

        public override String ToString()
        {
            return String.Format("[Playlist: {0}, {1}]", this.Author, this.Name);
        }

        public static Playlist FromXMLElement(XMLElement playlistElement, String id)
        {
            Playlist playlist = new Playlist();

		    /* Get "change" element. */
		    XMLElement changeElement = playlistElement.GetChild("next-change").GetChild("change");
    		
		    /* Set id. */
		    playlist.Id = id;
    		
		    /* Set author. */
		    playlist.Author = changeElement.GetChildText("user");
    		
		    /* Set name. */
		    playlist.Name = changeElement.GetChild("ops").GetChildText("name");
    		
		    /* Get items (comma separated list). */
		    if(changeElement.GetChild("ops").HasChild("add"))
            {
			    String items = changeElement.GetChild("ops").GetChild("add").GetChildText("items");
    			
			    /* Add track items. */
                foreach (String trackId in items.Split(new Char[] { ',' }))
                {
                    playlist.Tracks.Add(new Track(trackId.Trim().Substring(0, 32), "", null, null));
                }
		    }
    		
		    /* Get "version" element. */
		    XMLElement versionElement = playlistElement.GetChild("next-change").GetChild("version");
    		
		    /* Split version string into parts. */
		    String[] parts = versionElement.Text.Split(new Char[] { ',' });
    		
		    /* Set values. */
		    playlist.Revision = Int64.Parse(parts[0]);
            playlist._checksum = Int64.Parse(parts[2]);
		    playlist.Collaborative = (Int32.Parse(parts[3]) == 1);
    		
		    return playlist;
        }

        public static Playlist FromResult(String name, String author, Result result)
        {
		    Playlist playlist = new Playlist();
    		
		    playlist.Name   = name;
		    playlist.Author = author;
    		
		    foreach (Track track in result.Tracks)
			    playlist.Tracks.Add(track);
    		
		    return playlist;
	    }

        #endregion


        #region construction

        public Playlist() : this(null, null, null, false)
        {
        }

        public Playlist(String id, String name, String author, Boolean collaborative)
        {
            this.Id = id;
            this.Name = name;
            this.Author = author;
            this.Tracks = new List<Track>();
            this.Revision = -1;
            this._checksum = -1;
            this.Collaborative = collaborative;
        }

        #endregion


        #region IEnumerable<Track> Members

        public IEnumerator<Track> GetEnumerator()
        {
            return this.Tracks.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.Tracks.GetEnumerator();
        }

        #endregion
    }
}
