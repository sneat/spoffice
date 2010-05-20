using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class Track : Media
    {
        #region fields

        private Int32 _year = -1;
        private Int32 _trackNumber = -1;
        private Int32 _length = -1;

        #endregion


        #region properties

        public String Title { get; set; }
        public Artist Artist { get; set; }
        public Album Album { get; set; }
        public List<File> Files { get; set; }
        public String Cover { get; set; }
        public List<Track> SimilarTracks { get; set; }

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

        public Int32 TrackNumber
        {
            get { return _trackNumber; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Expecting a track number greater than zero.");
                _trackNumber = value;
            }
        }

        public Int32 Length
        {
            get { return _length; }
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Expecting a length greater than zero.");
                _length = value;
            }
        }

        #endregion


        #region methods

        public void AddFile(File file)
        {
            this.Files.Add(file);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Track)
            {
                Track t = obj as Track;
                if (this.Id == t.Id)
                    return true;
                foreach (String rId in this.Redirects)
                {
                    if (rId == t.Id)
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
            return String.Format("[Track: {0}, {1}, {2}]", this.Artist, this.Album, this.Title);
        }

        public static new Track FromXMLElement(XMLElement trackElement)
        {
		    Track track = new Track();
    		
		    /* Set id. */
		    if(trackElement.HasChild("id"))
			    track.Id = trackElement.GetChildText("id");
    		
		    /* Set title. */
		    if(trackElement.HasChild("title"))
			    track.Title = trackElement.GetChildText("title");
    		
		    /* Set artist. */
		    if(trackElement.HasChild("artist-id") && trackElement.HasChild("artist"))
            {
			    track.Artist = new Artist(trackElement.GetChildText("artist-id"),
				                    trackElement.GetChildText("artist"));
		    }
    		
		    /* Set album. */
		    if(trackElement.HasChild("album-id") && trackElement.HasChild("album"))
            {
			    track.Album = new Album(
				    trackElement.GetChildText("album-id"),
				    trackElement.GetChildText("album"),
				    track.Artist);
		    }
    		
		    /* Set year. */
		    if(trackElement.HasChild("year"))
            {
                Int32 year = -1;
                if (IntegerUtilities.TryParse(trackElement.GetChildText("year"), out year))
                    track.Year = year;
		    }
    		
		    /* Set track number. */
		    if(trackElement.HasChild("track-number"))
			    track.TrackNumber = Int32.Parse(trackElement.GetChildText("track-number"));
    		
		    /* Set length. */
		    if(trackElement.HasChild("length"))
			    track.Length = Int32.Parse(trackElement.GetChildText("length"));
    		
		    /* Set files. */
		    if(trackElement.HasChild("files"))
            {
			    foreach (XMLElement fileElement in trackElement.GetChild("files").GetChildren())
                {
				    File file = new File(fileElement.GetAttribute("id"), fileElement.GetAttribute("format"));
				    track.Files.Add(file);
			    }
		    }
    		
		    /* Set cover. */
		    if(trackElement.HasChild("cover"))
            {
			    String value = trackElement.GetChildText("cover");

			    if(!String.IsNullOrEmpty(value))
				    track.Cover = value;
		    }
    		
		    /* Set popularity. */
		    if(trackElement.HasChild("popularity"))
			    track.Popularity = Single.Parse(trackElement.GetChildText("popularity"));
    		
		    //FIXME: TODO: similar-tracks
    		
		    return track;
	    }

        #endregion


        #region construction

        public Track()
        {
            this.Title = null;
            this.Artist = null;
            this.Album = null;
            this.Files = new List<File>();
            this.Cover = null;
            this.SimilarTracks = new List<Track>();
        }

        public Track(String id) : this(id, null, null, null)
        {
        }

        public Track(String id, String title, Artist artist, Album album) : base(id)
        {
            this.Title = null;
            this.Artist = null;
            this.Album = null;
            this.Files = new List<File>();
            this.Cover = null;
            this.SimilarTracks = new List<Track>();
        }

        #endregion
    }
}
