using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using SharpSpotLib.Util;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models.Spotify
{
    public class MusicSearch
    {
        public const string ARTIST_SEARCH_URL = "http://ws.spotify.com/search/1/artist?q={0}";
        public const string ALBUM_SEARCH_URL = "http://ws.spotify.com/search/1/album?q={0}";
        public const string TRACK_SEARCH_URL = "http://ws.spotify.com/search/1/track?q={0}";
        public static XNamespace ns = "http://www.spotify.com/ns/music/1";

        public static List<ArtistOutput> SearchForArtist(string query)
        {
            MetadataApiParser parser = new MetadataApiParser();

            XDocument data = XDocument.Load(String.Format(ARTIST_SEARCH_URL, query));

            var artists = from item in data.Descendants(ns + "artists").Elements(ns + "artist")
                          select parser.ParseArtist(item);

            return artists.ToList();
        }
        public static List<TrackOutput> SearchForTrack(string query)
        {
            MetadataApiParser parser = new MetadataApiParser();
            XDocument data = XDocument.Load(String.Format(TRACK_SEARCH_URL, query));

            var tracks = from item in data.Descendants(ns + "tracks").Elements(ns + "track")
                         select parser.ParseTrack(item);
            return tracks.ToList();
        }
        public static List<AlbumOutput> SearchForAlbum(string query)
        {
            MetadataApiParser parser = new MetadataApiParser();
            XDocument data = XDocument.Load(String.Format(ALBUM_SEARCH_URL, query));

            var albums = from item in data.Descendants(ns + "albums").Elements(ns + "album")
                         select parser.ParseAlbum(item);

            return albums.ToList();
        }
    }
}
