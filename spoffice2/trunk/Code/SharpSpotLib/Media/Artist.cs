using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class Artist : Media
    {
        #region fields

        #endregion


        #region properties

        public String Name { get; set; }
        public Image Portrait { get; set; }
        public List<String> Genres { get; set; }
        public List<String> YearsActive { get; set; }
        public List<Biography> Bios { get; set; }
        public List<Album> Albums { get; set; }
        public List<Artist> SimilarArtists { get; set; }

        public String URI
        {
            get
            {
                return "spotify:artist:" + SpotifyURI.ToBase62(this.Id);
            }
        }

        public String Link
        {
            get
            {
                return "http://open.spotify.com/artist/" + SpotifyURI.ToBase62(this.Id);
            }
        }

        #endregion


        #region methods

        public override Boolean Equals(Object obj)
        {
            if (obj is Artist)
            {
                Artist a = obj as Artist;
                if (this.Id == a.Id)
                    return true;
                foreach (String rId in this.Redirects)
                {
                    if (rId == a.Id)
                        return true;
                }
            }
            return false;
        }

        public override Int32 GetHashCode()
        {
            return (this.Id != null) ? this.Id.GetHashCode() : 0;
        }

        public override String ToString()
        {
            return String.Format("[Artist: {0}]", this.Name);
        }

        public static new Artist FromXMLElement(XMLElement artistElement)
        {
            /* Create an empty artist object. */
		    Artist artist = new Artist();
    		
		    /* Set identifier. */
		    if(artistElement.HasChild("id"))
			    artist.Id = artistElement.GetChildText("id");
    		
		    /* Set name. */
		    if(artistElement.HasChild("name"))
			    artist.Name = artistElement.GetChildText("name");
    		
		    /* Set portrait. */
		    if(artistElement.HasChild("portrait") && artistElement.GetChild("portrait").HasChild("id"))
            {
			    String id = artistElement.GetChild("portrait").GetChildText("id");
			    if (!String.IsNullOrEmpty(id))
				    artist.Portrait = new Image(id, -1, -1);
		    }
    		
		    /* Set popularity. */
		    if (artistElement.HasChild("popularity"))
                artist.Popularity = Single.Parse(artistElement.GetChildText("popularity"));
    		
		    /* Set similar artists. */
		    if (artistElement.HasChild("similar-artists"))
            {
			    foreach (XMLElement similarArtistElement in artistElement.GetChild("similar-artists").GetChildren())
				    artist.SimilarArtists.Add(Artist.FromXMLElement(similarArtistElement));
		    }
    		
		    //FIXME: TODO: bios, genres, years-active, albums, ...
    		
		    return artist;
        }

        #endregion


        #region construction

        public Artist()
        {
            this.Name = null;
            this.Portrait = null;
            this.Genres = new List<String>();
            this.YearsActive = new List<String>();
            this.Bios = new List<Biography>();
            this.Albums = new List<Album>();
            this.SimilarArtists = new List<Artist>();
        }

        public Artist(String id, String name) : base(id)
        {
            this.Name = name;
            this.Portrait = null;
            this.Genres = new List<String>();
            this.YearsActive = new List<String>();
            this.Bios = new List<Biography>();
            this.SimilarArtists = new List<Artist>();
            this.Albums = new List<Album>();
        }

        public Artist(String id) : this(id, null)
        {
        }

        #endregion
    }
}
