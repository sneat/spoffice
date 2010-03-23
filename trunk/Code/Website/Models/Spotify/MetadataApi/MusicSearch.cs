using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Collections.Generic;
using SharpSpotLib.Util;

namespace Spoffice.Website.Models.Spotify.MetadataApi
{
    public class MusicSearch
    {
        public const string ARTIST_SEARCH_URL = "http://ws.spotify.com/search/1/artist?q={0}";
        public const string ALBUM_SEARCH_URL = "http://ws.spotify.com/search/1/album?q={0}";
        public const string TRACK_SEARCH_URL = "http://ws.spotify.com/search/1/track?q={0}";
        public static XNamespace ns = "http://www.spotify.com/ns/music/1";

        public static List<ArtistNode> SearchForArtist(string query)
        {
            XDocument data = XDocument.Load(String.Format(ARTIST_SEARCH_URL, query));

            var artists = from item in data.Descendants(ns + "artists").Elements(ns + "artist")
                          select new ArtistNode(item);

            return artists.ToList();
        }
        public static List<TrackNode> SearchForTrack(string query)
        {
            XDocument data = XDocument.Load(String.Format(TRACK_SEARCH_URL, query));

            var tracks = from item in data.Descendants(ns + "tracks").Elements(ns + "track")
                          select new TrackNode(item);
            return tracks.ToList();
        }
        public static List<AlbumNode> SearchForAlbum(string query)
        {
            XDocument data = XDocument.Load(String.Format(ALBUM_SEARCH_URL, query));

            var albums = from item in data.Descendants(ns + "albums").Elements(ns + "album")
                         select new AlbumNode(item);

            return albums.ToList();
        }
    }
}
