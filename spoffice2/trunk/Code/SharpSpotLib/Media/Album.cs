using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class Album : Media
    {
        #region fields

        private String _cover = null;
        private Int32 _year = -1;

        #endregion


        #region properties

        public String Name { get; set; }
        public Artist Artist { get; set; }
        public String Type { get; set; }
        public String Review { get; set; }
        public List<Disc> Discs { get; set; }
        public List<Album> SimilarAlbums { get; set; }

        public String Cover
        {
            get { return _cover; }
            set 
            {
                if (value == null || value.Length != 40 || !Hex.IsHex(value))
                    throw new ArgumentException("Expecting a 40-character hex string.");
                _cover = value;
            }
        }

        public Int32 Year
        {
            get { return _year; }
            set
            {
                if (value < 0)
			        throw new ArgumentException("Expecting a positive year.");
                _year = value;
            }
        }

        public List<Track> Tracks
        {
            get
            {
                List<Track> tracks = new List<Track>();
                foreach (Disc disc in this.Discs)
                    tracks.AddRange(disc.Tracks);
		        return tracks;
            }
        }

        public String URI
        {
            get
            {
                return "spotify:album:" + SpotifyURI.ToBase62(this.Id);
            }
        }

        public String Link
        {
            get
            {
                return "http://open.spotify.com/album/" + SpotifyURI.ToBase62(this.Id);
            }
        }

        #endregion


        #region methods

        public void SetTracks(List<Disc> discs)
        {
            this.Discs = discs;
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Album)
            {
                Album a = obj as Album;
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
            return String.Format("[Album: {0}, {1}]", this.Artist, this.Name);
        }

        public static new Album FromXMLElement(XMLElement albumElement)
        {
            Album album = new Album();
		
		    /* Set identifier. */
		    if(albumElement.HasChild("id"))
			    album.Id = albumElement.GetChildText("id");

		    /* Set name. */
		    if(albumElement.HasChild("name"))
			    album.Name = albumElement.GetChildText("name");

		    /* Set artist. */
		    if(albumElement.HasChild("artist-id") && (albumElement.HasChild("artist") || albumElement.HasChild("artist-name")))
            {
			    album.Artist = new Artist(albumElement.GetChildText("artist-id"),
				    albumElement.HasChild("artist") ? albumElement.GetChildText("artist"): albumElement.GetChildText("artist-name"));
		    }
		
		    /* Set cover. */
		    if(albumElement.HasChild("cover"))
            {
			    String value = albumElement.GetChildText("cover");
			    if(!String.IsNullOrEmpty(value))
				    album.Cover = value;
		    }
		
		    /* Set year. */
		    if(albumElement.HasChild("year"))
			    album.Year = Int32.Parse(albumElement.GetChildText("year"));
		
		    /* Set popularity. */
		    if(albumElement.HasChild("popularity"))
			    album.Popularity = Single.Parse(albumElement.GetChildText("popularity"));
		
		    /* Set tracks. */
		    if(albumElement.HasChild("discs")){
			    /* Loop over discs. */
			    foreach (XMLElement discElement in albumElement.GetChild("discs").GetChildren("disc"))
                {
				    Disc disc = new Disc(Int32.Parse(discElement.GetChildText("disc-number")),
					                        discElement.GetChildText("name"));
    				
				    /* Loop over tracks of current disc. */
				    foreach (XMLElement trackElement in discElement.GetChildren("track"))
                    {
					    /* Parse track element. */
					    Track track = Track.FromXMLElement(trackElement);
    					
					    /* Also add artist and album information to track. */
                        track.Artist = album.Artist;
					    track.Album = album;
					    track.Cover = album.Cover;
    					
					    /* Add track to list of tracks. */
					    disc.Tracks.Add(track);
				    }
    				
				    album.Discs.Add(disc);
			    }
		    }		
		
            //FIXME: TODO: album-type, copyright, discs, ...
		
		    return album;
        }

        #endregion


        #region construction

        public Album()
        {
            this.Name = null;
            this.Artist = null;
            this.Type = null;
            this.Review = null;
            this.Discs = new List<Disc>();
            this.SimilarAlbums = new List<Album>();
        }

        public Album(String id) : this(id, null, null)
        {
        }

        public Album(String id, String name, Artist artist) : base(id)
        {
            this.Name = name;
            this.Artist = artist;
            this.Type = null;
            this.Review = null;
            this.Discs = new List<Disc>();
            this.SimilarAlbums = new List<Album>();
        }

        #endregion
    }
}
