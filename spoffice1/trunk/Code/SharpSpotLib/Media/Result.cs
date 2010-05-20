using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class Result
    {
        #region fields

        #endregion


        #region properties

        public String Query { get; set; }
        public String Suggestion { get; set; }
        public Int32 TotalArtists { get; set; }
        public Int32 TotalAlbums { get; set; }
        public Int32 TotalTracks { get; set; }
        public List<Artist> Artists { get; set; }
        public List<Album> Albums { get; set; }
        public List<Track> Tracks { get; set; }

        #endregion


        #region methods

        public void AddArtist(Artist artist)
        {
            this.Artists.Add(artist);
        }

        public void AddAlbum(Album album)
        {
            this.Albums.Add(album);
        }

        public void AddTrack(Track track)
        {
            this.Tracks.Add(track);
        }

        public override Boolean Equals(Object obj)
        {
            if (obj is Result)
                return this.Query.Equals((obj as Result).Query);
            return false;
        }

        public override Int32 GetHashCode()
        {
            return (this.Query != null) ? this.Query.GetHashCode() : 0;
        }

        public static Result FromXMLElement(XMLElement resultElement)
        {
            return FromXMLElement("", resultElement);
        }

        public static Result FromXMLElement(String query, XMLElement resultElement){
		    /* Create Result object. */
		    Result result = new Result();
    		
		    /* Set query. */
		    result.Query = query;
    		
		    /* Set suggestion. */
		    if(resultElement.HasChild("did-you-mean"))
			    result.Suggestion = resultElement.GetChildText("did-you-mean");
    		
		    /* Set result quantities.*/
		    if(resultElement.HasChild("total-artists") &&
			    resultElement.HasChild("total-albums") &&
			    resultElement.HasChild("total-tracks"))
            {
			    result.TotalArtists = Int32.Parse(resultElement.GetChildText("total-artists"));
			    result.TotalAlbums  = Int32.Parse(resultElement.GetChildText("total-albums"));
			    result.TotalTracks  = Int32.Parse(resultElement.GetChildText("total-tracks"));
		    }
    		
		    /* Get artists. */
		    if(resultElement.HasChild("artists"))
            {
			    foreach (XMLElement artistElement in resultElement.GetChild("artists").GetChildren())
                {
				    result.Artists.Add(Artist.FromXMLElement(artistElement));
			    }
		    }
    		
		    /* Get albums. */
		    if(resultElement.HasChild("albums")){
			    foreach (XMLElement albumElement in resultElement.GetChild("albums").GetChildren())
                {
				    result.Albums.Add(Album.FromXMLElement(albumElement));
			    }
		    }
    		
		    /* Get tracks. */
		    if(resultElement.HasChild("tracks"))
            {
			    foreach (XMLElement trackElement in resultElement.GetChild("tracks").GetChildren())
                {
				    result.Tracks.Add(Track.FromXMLElement(trackElement));
			    }
		    }
    		
		    /* Return result. */
		    return result;
	    }

        #endregion


        #region construction

        public Result()
        {
            this.Query = null;
            this.Suggestion = null;
            this.TotalArtists = 0;
            this.TotalAlbums = 0;
            this.TotalTracks = 0;
            this.Artists = new List<Artist>();
            this.Albums = new List<Album>();
            this.Tracks = new List<Track>();
        }

        #endregion
    }
}
