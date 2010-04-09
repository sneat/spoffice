using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Spoffice.Website.Services;
using Spoffice.Website.Models.Output;
using System.Globalization;
using System.IO;
using System.Web;
using SharpSpotLib.Util;

namespace Spoffice.Spotify
{
    public class MusicBrowser : IMusicBrowser
    {
        public const string ARTIST_SEARCH_URL = "http://ws.spotify.com/search/1/artist?q={0}";
        public const string ALBUM_SEARCH_URL = "http://ws.spotify.com/search/1/album?q={0}";
        public const string TRACK_SEARCH_URL = "http://ws.spotify.com/search/1/track?q={0}";
        public static XNamespace ns = "http://www.spotify.com/ns/music/1";

        #region IMusicBrowser Members

        public List<ArtistOutput> SearchForArtist(string query)
        {
            XDocument data = XDocument.Load(String.Format(ARTIST_SEARCH_URL, query));

            var artists = from item in data.Descendants(ns + "artists").Elements(ns + "artist")
                          select ParseArtist(item);

            return artists.ToList();
        }

        public List<AlbumOutput> SearchForAlbum(string query)
        {
            XDocument data = XDocument.Load(String.Format(ALBUM_SEARCH_URL, query));

            var albums = from item in data.Descendants(ns + "albums").Elements(ns + "album")
                         select ParseAlbum(item);

            return albums.ToList();
        }

        public List<TrackOutput> SearchForTrack(string query)
        {
            XDocument data = XDocument.Load(String.Format(TRACK_SEARCH_URL, query));

            var tracks = from item in data.Descendants(ns + "tracks").Elements(ns + "track")
                         select ParseTrack(item);

            return tracks.ToList();
        }

        public ArtistOutput GetArtistById(Guid id)
        {
            string publicId = ConvertPrivateToPublic(id);
            return ParseArtist(GetData(publicId, "http://ws.spotify.com/lookup/1/?uri={0}&extras=albumdetail", "artist", String.Format("{0}:{1}:{2}", "spotify", "artist", publicId)));
        }

        public AlbumOutput GetAlbumById(Guid id)
        {
            string publicId = ConvertPrivateToPublic(id);
            return ParseAlbum(GetData(publicId, "http://ws.spotify.com/lookup/1/?uri={0}&extras=trackdetail", "album", String.Format("{0}:{1}:{2}", "spotify", "album", publicId)));
        }

        public TrackOutput GetTrackById(Guid id)
        {
            string publicId = ConvertPrivateToPublic(id);
            return ParseTrack(GetData(publicId, "http://ws.spotify.com/lookup/1/?uri={0}", "track", String.Format("{0}:{1}:{2}", "spotify", "track", publicId)), id);
        }

        #endregion

        #region artist parsing
        public ArtistOutput ParseArtist(XDocument artist)
        {
            XElement root = artist.Element(ns + "artist");
            return ParseArtist(root);
        }
        public ArtistOutput ParseArtist(XElement artist)
        {
            ArtistOutput output = new ArtistOutput();
            output.Id = ParseHref(artist);
            output.MusicBrainzId = ParseMusicBrainz(artist);

            XElement xname = artist.Element(ns + "name");
            if (xname != null)
                output.Name = xname.Value;

            XElement xalbums = artist.Element(ns + "albums");
            if (xalbums != null)
            {
                var albums = from item in xalbums.Elements(ns + "album")
                             select ParseAlbum(item);

                output.Albums = albums.ToList();
            }
            return output;
        }
        #endregion

        #region track parsing
        public TrackOutput ParseTrack(XDocument track, Guid id)
        {
            return ParseTrack(track.Element(ns + "track"), id);
        }
        public TrackOutput ParseTrack(XElement track)
        {
            return ParseTrack(track, Guid.Empty);
        }
        public TrackOutput ParseTrack(XElement track, Guid id)
        {
            TrackOutput output = new TrackOutput();

            output.Id = ParseHref(track);
            if (id != Guid.Empty)
            {
                output.Id = id;
            }

            output.MusicBrainzId = ParseMusicBrainz(track);

            XElement xname = track.Element(ns + "name");
            if (xname != null)
                output.Title = xname.Value;

            XElement xartist = track.Element(ns + "artist");
            if (xartist != null)
                output.Artist = ParseArtist(xartist);

            XElement xalbum = track.Element(ns + "album");
            if (xalbum != null)
                output.Album = ParseAlbum(xalbum);

            try
            {
                XElement xlength = track.Element(ns + "length");
                if (xlength != null)
                {
                    string asString = xlength.Value;
                    double asSeconds;
                    Double.TryParse(asString, NumberStyles.Number, CultureInfo.GetCultureInfo("en-GB"), out asSeconds);
                    output.Length = Convert.ToInt32(asSeconds) * 1000;
                }
            }
            catch (Exception e)
            {
            }

            return output;

        }

        #endregion

        #region album parsing
        public AlbumOutput ParseAlbum(XDocument album)
        {
            return ParseAlbum(album.Element(ns + "album"));
        }
        public AlbumOutput ParseAlbum(XElement album)
        {
            AlbumOutput output = new AlbumOutput();
            output.Id = ParseHref(album);
            output.MusicBrainzId = ParseMusicBrainz(album);

            XElement xname = album.Element(ns + "name");
            if (xname != null)
                output.Name = xname.Value;

            XElement xavailable = album.Element(ns + "availability");
            if (xavailable != null)
            {
                XElement xterritories = xavailable.Element(ns + "territories");
                if (xterritories != null)
                    output.IsAvailable = xterritories.Value.Contains("GB");
            }

            XElement xartist = album.Element(ns + "artist");
            if (xartist != null)
                output.Artist = ParseArtist(xartist);


            XElement xtracks = album.Element(ns + "tracks");
            if (xtracks != null)
            {
                var tracks = from item in xtracks.Elements(ns + "track")
                             select ParseTrack(item);

                output.Tracks = tracks.ToList();
            }
            return output;
        }
        #endregion

        private XDocument GetData(string id, string lookupUrl, string folder, string uri)
        {
            string cachePath = System.Web.HttpContext.Current.Server.MapPath(String.Format("~/{1}{0}{2}{0}{3}{0}{4}.xml", Path.DirectorySeparatorChar, "Cache", "requests", folder, id));
            XDocument data;
            if (File.Exists(cachePath) && File.GetLastWriteTime(cachePath).AddMonths(1) > DateTime.Now)
            {
                data = XDocument.Load(cachePath);
            }
            else
            {
                data = XDocument.Load(String.Format(lookupUrl, uri));
                CreateFolder(cachePath);
                data.Save(cachePath);
            }
            return data;
        }
        protected Guid ParseMusicBrainz(XElement root)
        {
            try
            {
                XElement xmusicbrainz = root.Elements(ns + "id").Where(x => x.Attribute("type").Value == "mbid").SingleOrDefault();
                if (xmusicbrainz != null)
                    return new Guid(xmusicbrainz.Value);
            }
            catch (Exception e)
            {
            }
            return Guid.Empty;
        }

        protected Guid ParseHref(XElement root)
        {
            XAttribute xhref = root.Attribute("href");
            if (xhref != null)
            {
                return ConvertPublicToPrivate(new SpotifyURI(xhref.Value).Id);
            }
            return Guid.Empty;
        }
        private void CreateFolder(string path)
        {
            if (!Directory.GetParent(path).Exists)
            {
                CreateFolder(Directory.GetParent(path).FullName);
            }
            if (Path.GetFileName(path) != String.Empty)
            {
                path = new FileInfo(path).Directory.FullName;
            }
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
        private Guid ConvertPublicToPrivate(string publicId)
        {
            return new Guid(SpotifyURI.ToHex(publicId));
        }
        private string ConvertPrivateToPublic(Guid privateId)
        {
            return SpotifyURI.ToBase62(privateId.ToString("N"));
        }
    }
}
