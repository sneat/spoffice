using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Xml;
using System.Xml.XPath;
using SharpSpotLib.Exceptions;

namespace SharpSpotLib.Media.Parser
{
    internal class XMLMediaParser
    {
        #region constants

        private const Int32 SUPPORTED_RESULT_VERSION = 1;
	    private const Int32 SUPPORTED_ARTIST_VERSION = 1;
        private const Int32 SUPPORTED_ALBUM_VERSION = 1;

        #endregion


        #region fields

        private XmlReader _reader;

        #endregion


        #region properties

        #endregion


        #region methods

        private Object Parse() 
        {
		    String name;
		
		    /* Check if reader is currently on a start element. */
		    if (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;

                switch (name)
                {
                    case "result":
                    case "toplist":
                        return this.ParseResult();
                    case "artist":
                        return this.ParseArtist();
                    case "album":
                        return this.ParseAlbum();
                    case "track":
                        return this.ParseTrack();
                    case "playlists":
                        return this.ParsePlaylistContainer();
                    case "playlist":
                        return this.ParsePlaylist();
                    default:
                        throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                }
		    }
		    throw new InvalidOperationException("Reader is not on a start element.");
	    }

        private Result ParseResult()
        {
		    Result result = new Result();
		    String name;

		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (_reader.NodeType == XmlNodeType.Element)
            {
			    name = this._reader.Name;
    			
			    /* Process depending on element name. */
                switch (name)
                {
                    case "version":
                        Int32 version = this.GetElementInteger();
                        if (version > SUPPORTED_RESULT_VERSION)
                            throw new XMLMediaParseException("Unsupported <result> version " + version);
                        break;
                    case "did-you-mean":
                        result.Suggestion = this.GetElementString();
                        break;
                    case "total-artists":
                        result.TotalArtists = this.GetElementInteger();
                        break;
                    case "total-albums":
                        result.TotalAlbums = this.GetElementInteger();
                        break;
                    case "total-tracks":
                        result.TotalTracks = this.GetElementInteger();
                        break;
                    case "artists":
                        result.Artists = this.ParseArtists();
                        break;
                    case "albums":
                        result.Albums = this.ParseAlbums();
                        break;
                    case "tracks":
                        result.Tracks = this.ParseTracks();
                        break;
                    default:
                        throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                }
		    }
    		
		    return result;
	    }


        private List<Artist> ParseArtists()
        {
		    List<Artist> artists = new List<Artist>();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
		    while (this._reader.NodeType == XmlNodeType.Element)
            {
			    name = this._reader.Name;
    			
			    /* Process depending on element name. */
			    if (name == "artist")
                {
				    artists.Add(this.ParseArtist());
			    }
			    else
                {
				    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
			    }
		    }
            this._reader.Read();
    		
		    return artists;
	    }

        private List<Album> ParseAlbums()
        {
		    List<Album> albums = new List<Album>();
		    String name;
    				
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;
    			
			    /* Process depending on element name. */
			    if (name == "album")
                {
				    albums.Add(this.ParseAlbum());
			    }
			    else
                {
				    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
			    }
		    }
            this._reader.Read();
    		
		    return albums;
	    }

        private List<Track> ParseTracks()
        {
		    List<Track> tracks = new List<Track>();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;
    			
			    /* Process depending on element name. */
			    if (name == "track")
                {
				    tracks.Add(this.ParseTrack());
			    }
			    else
                {
				    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
			    }
		    }
            this._reader.Read();
    		
		    return tracks;
	    }

        private Artist ParseArtist()
        {
		    Artist artist = new Artist();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
		    while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;

                /* Process depending on element name. */
                switch (name)
                {
                    case "version":
                        Int32 version = this.GetElementInteger();
                        if(version > SUPPORTED_ARTIST_VERSION)
					        throw new XMLMediaParseException("Unsupported <album> version ");
                        break;
                    case "id":
                        /* TODO: handle different ID types. */
				        if (this.GetAttributeString("type") == null)
					        artist.Id = this.GetElementString();
                        break;
                    case "redirect":
                        artist.AddRedirect(this.GetElementString());
                        break;
                    case "name":
                        artist.Name = this.GetElementString();
                        break;
                    case "portrait":
                        artist.Portrait = this.ParseImage();
                        break;
                    case "genres":
                        String[] genres = this.GetElementString().Split(new Char[] { ',' });
    				    artist.Genres = new List<String>(genres);
                        break;
                    case "years-active":
                        String[] years = this.GetElementString().Split(new Char[] { ',' });
    				    artist.YearsActive = new List<String>(years);
                        break;
                    case "popularity":
                        artist.Popularity = this.GetElementFloat();
                        break;
                    case "bios":
                        artist.Bios = this.ParseBios();
                        break;
                    case "similar-artists":
                        artist.SimilarArtists = this.ParseArtists();
                        break;
                    case "albums":
                        artist.Albums = this.ParseAlbums();
                        break;
                    case "restrictions":
                        artist.Restrictions = this.ParseRestrictions();
                        break;
                    case "external-ids":
                        artist.ExternalIds = this.ParseExternalIds();
                        break;
                    default:
                        throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                }

		    }
            this._reader.Read();

		    return artist;
	    }


        private Album ParseAlbum()
        {
		    Album  album = new Album();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;
    			
			    /* Process depending on element name. */
                switch (name)
                {
                    case "version":
                        Int32 version = this.GetElementInteger();
                        if (version > SUPPORTED_ALBUM_VERSION)
                            throw new XMLMediaParseException("Unsupported <album> version " + version);
                        break;
                    case "id":
                        /* TODO: handle different ID types. */
				        if (this.GetAttributeString("type") == null)
					        album.Id = this.GetElementString();
				        break;
                    case "redirect":
                        album.AddRedirect(this.GetElementString());
                        break;
                    case "name":
                        album.Name = this.GetElementString();
                        break;
                    case "artist":
                    case "artist-name":
                        Artist artist1 = (album.Artist != null) ? album.Artist : new Artist();
                        artist1.Name = this.GetElementString();
                        album.Artist = artist1;
                        break;
                    case "artist-id":
                        Artist artist2 = (album.Artist != null) ? album.Artist : new Artist();
                        artist2.Id = this.GetElementString();
                        album.Artist = artist2;
                        break;
                    case "album-type":
                        album.Type = this.GetElementString();
                        break;
                    case "cover":
                        String cover = this.GetElementString();
                        if (!String.IsNullOrEmpty(cover))
                            album.Cover = cover;
                        break;
                    case "popularity":
                        album.Popularity = this.GetElementFloat();
                        break;
                    case "review":
                        album.Review = this.GetElementString();
                        break;
                    case "year":
                    case "released":
                        album.Year = this.GetElementInteger();
                        break;
                    case "copyright": /* TODO: currently skipped. */
                        this._reader.Read();
                        while (this._reader.NodeType == XmlNodeType.Element)
                        {
                            name = this._reader.Name;
                            switch (name)
                            {
                                case "c":
                                    this.GetElementString(); //Skip
                                    break;
                                case "p":
                                    this.GetElementString(); //Skip
                                    break;
                                default:
                                    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                            }
                        }
                        this._reader.Read();
                        break;
                    case "links": /* TODO: currently skipped. */
                        this.SkipLinks();
                        break;
                    case "restrictions":
                        album.Restrictions = this.ParseRestrictions();
                        break;
                    case "availability": /* TODO: currently skipped. */
                        this.SkipAvailability();
                        break;
                    case "allowed": /* Seems to be deprecated. */
                        this.GetElementString(); //Skip
                        break;
                    case "forbidden": /* Seems to be deprecated. */
                        this.GetElementString(); //Skip
                        break;
                    case "discs":
                        List<Disc> discs = new List<Disc>();
                        this._reader.Read();
                        while (this._reader.NodeType == XmlNodeType.Element)
                        {
                            name = this._reader.Name;
                            if (name == "disc")
                            {
                                List<Track> tracks = new List<Track>();
					            Disc disc = new Disc();
                                this._reader.Read();
                                while (this._reader.NodeType == XmlNodeType.Element)
                                {
                                    name = this._reader.Name;
                                    switch (name)
                                    {
                                        case "disc-number":
                                            disc.Number = this.GetElementInteger();
                                            break;
                                        case "name":
                                            disc.Name = this.GetElementString();
                                            break;
                                        case "track":
                                            Track track = this.ParseTrack();
                                            track.Album = album;
                                            tracks.Add(track);
                                            break;
                                        default:
                                            throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                                    }
                                }
                                this._reader.Read();
                                /* Set disc tracks. */
					            disc.Tracks = tracks;
					            discs.Add(disc);
                            }
                            else
                                throw new XMLMediaParseException("Unexpected element '<" + name + ">'");

                        }
                        this._reader.Read();
                        album.Discs = discs;
                        break;
                    case "similar-albums":
                        List<Album> similarAlbums = new List<Album>();
                        this._reader.Read();
                        while (this._reader.NodeType == XmlNodeType.Element)
                        {
                            name = this._reader.Name;
                            if (name == "id")
                            {
                                similarAlbums.Add(new Album(this.GetElementString()));
                            }
                            else
                                throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                        }
                        this._reader.Read();
                        album.SimilarAlbums = similarAlbums;
                        break;
                    case "external-ids":
                        album.ExternalIds = this.ParseExternalIds();
                        break;
                    default:
                        throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                }
		    }
            this._reader.Read();
    		
		    return album;
	    }

        private Track ParseTrack()
        {
		    Track  track = new Track();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;

                switch (name)
                {
                    case "id":
                        /* TODO: handle different ID types. */
                        track.Id = this.GetElementString();
                        break;
                    case "redirect":
                        track.AddRedirect(this.GetElementString());
                        break;
                    case "title":
                    case "name":
                        track.Title = this.GetElementString();
                        break;
                    case "artist":
                        Artist artist1 = (track.Artist != null) ? track.Artist : new Artist();
                        artist1.Name = this.GetElementString();
                        track.Artist = artist1;
                        break;
                    case "artist-id":
                        Artist artist2 = (track.Artist != null) ? track.Artist : new Artist();
                        artist2.Id = this.GetElementString();
                        track.Artist = artist2;
                        break;
                    case "album":
                        Album album1 = (track.Album != null) ? track.Album : new Album();
                        album1.Name = this.GetElementString();
                        track.Album = album1;
                        break;
                    case "album-id":
                        Album album2 = (track.Album != null) ? track.Album : new Album();
                        album2.Id = this.GetElementString();
                        track.Album = album2;
                        break;
                    case "album-artist":
                        Album album3 = (track.Album != null) ? track.Album : new Album();
                        Artist artist3 = (track.Artist != null) ? track.Artist : new Artist();
                        artist3.Name = this.GetElementString();
                        album3.Artist = artist3;
                        track.Album = album3;
                        break;
                    case "album-artist-id":
                        Album album4 = (track.Album != null) ? track.Album : new Album();
                        Artist artist4 = (track.Artist != null) ? track.Artist : new Artist();
                        artist4.Id = this.GetElementString();
                        album4.Artist = artist4;
                        track.Album = album4;
                        break;
                    case "year":
                        track.Year = this.GetElementInteger();
                        break;
                    case "track-number":
                        track.TrackNumber = this.GetElementInteger();
                        break;
                    case "length":
                        Int32 length = this.GetElementInteger();
                        if (length > 0)
                            track.Length = length;
                        break;
                    case "files":
                        track.Files = this.ParseFiles();
                        break;
                    case "links": /* TODO: currently skipped. */
                        this.SkipLinks();
                        break;
                    case "album-links": /* TODO: currently skipped. */
                        this.SkipLinks();
                        break;
                    case "cover":
                        track.Cover = this.GetElementString();
                        break;
                    case "popularity":
                        track.Popularity = this.GetElementFloat();
                        break;
                    case "restrictions":
                        track.Restrictions = this.ParseRestrictions();
                        break;
                    case "allowed": /* Seems to be deprecated. */
                        this.GetElementString(); /* Skip text. */
                        break;
                    case "forbidden": /* Seems to be deprecated. */
                        this.GetElementString(); /* Skip text. */
                        break;
                    case "similar-tracks":
                        List<Track> similarTracks = new List<Track>();
                        this._reader.Read();
                        while (this._reader.NodeType == XmlNodeType.Element)
                        {
                            name = this._reader.Name;
                            if (name == "id")
                            {
                                similarTracks.Add(new Track(this.GetElementString()));
                            }
                            else
                                throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                        }
                        this._reader.Read();
                        track.SimilarTracks = similarTracks;
                        break;
                    case "external-ids":
                        track.ExternalIds = this.ParseExternalIds();
                        break;
                    case "explicit":
                        this.GetElementString();
                        break;
                    default:
                        throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                }
		    }
            this._reader.Read();
    		
		    /* If album artist of this track is not yet set, then set it. */
		    if (track.Album != null && track.Album.Artist == null)
			    track.Album.Artist = track.Artist;
    		
		    return track;
	    }

        /** TODO: Implement. */
	    private PlaylistContainer ParsePlaylistContainer()
        {
		    return null;
	    }
	
	    /** TODO: Implement. */
	    private Playlist ParsePlaylist()
        {
		    return null;
	    }

        private Image ParseImage()
        {
		    Image image = new Image();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;
    			
			    /* Process depending on element name. */
                switch (name)
                {
                    case "id":
                        image.Id = this.GetElementString();
                        break;
                    case "width":
                        image.Width = this.GetElementInteger();
                        break;
                    case "height":
                        image.Height = this.GetElementInteger();
                        break;
                    default:
                        throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                }
		    }
    		
		    /* If the reader is not at an end element, it is at some character event. */
		    if (this._reader.NodeType != XmlNodeType.EndElement)
            {

			    /* Read image id from element text (special case). */
                image.Id = this._reader.ReadContentAsString();
    			
			    /* Skip to end element. */
                this._reader.Read(); //FIXME: Needed?
                throw new NotImplementedException("This needs to be tested"); //FIXME!
		    }

            this._reader.Read();
    		
		    return image;
	    }


        private List<Biography> ParseBios()
        {
		    List<Biography> bios = new List<Biography>();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;

                if (name == "bio")
                {
                    Biography bio = new Biography();
                    this._reader.Read();
                    while (this._reader.NodeType == XmlNodeType.Element)
                    {
                        name = this._reader.Name;
                        if (name == "text")
                            bio.Text = this.GetElementString();
                        else if (name == "portraits")
                        {
                            List<Image> portraits = new List<Image>();
                            this._reader.Read();
                            while (this._reader.NodeType == XmlNodeType.Element)
                            {
                                name = this._reader.Name;
                                if (name == "portrait")
                                    portraits.Add(this.ParseImage());
                                else
                                    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                            }
                            this._reader.Read();
                            bio.Portraits = portraits;
                        }
                        else
                            throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
                    }
                    this._reader.Read();
                    bios.Add(bio);
                }
                else
                    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
		    }
            this._reader.Read();
    		
		    return bios;
	    }

        private List<File> ParseFiles()
        {
		    List<File> files = new List<File>();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;

                if (name == "file")
                {
                    files.Add(new File(this.GetAttributeString("id"), this.GetAttributeString("format")));
                    
                    /* Skip to end element, since we only read the attributes. */
                    this._reader.Read();
                }
                else
                    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
		    }
            this._reader.Read();

		    return files;
	    }

        private List<Restriction> ParseRestrictions()
        {
		    List<Restriction> restrictions = new List<Restriction>();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;
    			
			    /* Process depending on element name. */
			    if (name == "restriction")
                {
				    restrictions.Add(new Restriction(
					    this.GetAttributeString("allowed"),
                        this.GetAttributeString("forbidden"),
                        this.GetAttributeString("catalogues")
				    ));
    				
				    /* Skip to end element since we only read the attributes. */
                    this._reader.Read();
			    }
			    else
				    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
		    }
            this._reader.Read();

		    return restrictions;
	    }

        private Dictionary<String, String> ParseExternalIds()
        {
            Dictionary<String, String> externalIds = new Dictionary<String, String>();
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;

			    /* Process depending on element name. */
			    if (name == "external-id")
                {
				    externalIds.Add(
					    this.GetAttributeString("type"), 
                        this.GetAttributeString("id"));
    				
				    /* Skip to end element since we only read the attributes. */
                    this._reader.Read();
			    }
			    else
				    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
		    }
            this._reader.Read();
    		
		    return externalIds;
	    }


        private void SkipLinks()
        {
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
		    this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;
    			
			    /* Process depending on element name. */
			    if (name == "link")
                {
				    /* Skip text. */
				    this.GetElementString();
			    }
			    else
                    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
		    }
            this._reader.Read();

	    }
    	
	    private void SkipAvailability()
        {
		    String name;
    		
		    /* Go to next element and check if it is a start element. */
            this._reader.Read();
            while (this._reader.NodeType == XmlNodeType.Element)
            {
                name = this._reader.Name;
    			
			    /* Process depending on element name. */
			    if (name == "territories")
                {
				    /* Skip text. */
				    this.GetElementString();
			    }
			    else
                    throw new XMLMediaParseException("Unexpected element '<" + name + ">'");
		    }
            this._reader.Read();

	    }



        private String GetAttributeString(String attribute)
        {
		    return this._reader.GetAttribute(attribute);
	    }

	    private String GetElementString()
        {
            try
            {
		        return this._reader.ReadElementContentAsString();
            }
            catch (Exception)
            {
                return null;
            }
	    }
	

	    private Int32 GetElementInteger()
        {
		    try
            {
                return this._reader.ReadElementContentAsInt();
		    }
		    catch(Exception)
            {
			    return 0;
		    }
	    }
	
	    private Single GetElementFloat()
        {
		    try
            {
                return (Single)this._reader.ReadElementContentAs(typeof(Single), null);
		    }
		    catch (Exception)
            {
			    return Single.NaN;
		    }
	    }
	
	    public static Object Parse(Byte[] xml, Encoding encoding)
        {
            try
            {
                XMLMediaParser parser = new XMLMediaParser(new MemoryStream(xml), encoding);
                return parser.Parse();
            }
            catch (XMLMediaParseException)
            {
                return null;
            }
            catch (Exception ex)
            {
                throw;
            }
	    }
	
	    public static Result ParseResult(Byte[] xml, Encoding encoding)
        {
            return Parse(xml, encoding) as Result;
	    }

	    public static Artist ParseArtist(Byte[] xml, Encoding encoding)
        {
            return Parse(xml, encoding) as Artist;
	    }
	

	    public static Album ParseAlbum(Byte[] xml, Encoding encoding)
        {
		    return Parse(xml, encoding) as Album;
	    }


        #endregion


        #region construction

        private XMLMediaParser(Stream stream, Encoding encoding)
        {
            //FIXME: Encoding ignored

            XmlReaderSettings settings = new XmlReaderSettings();
            settings.IgnoreComments = true;
            settings.IgnoreWhitespace = true;
            _reader = XmlReader.Create(stream, settings);
           /* XmlDocument doc = new XmlDocument();
            doc.Load(_reader);
            Console.Write(doc.OuterXml);*/
            while (_reader.Read())
            {
                if (_reader.NodeType == XmlNodeType.Element) //Root node found
                    return;
            }
            throw new XMLMediaParseException("Root node not found.");
	    }

        #endregion
    }
}
