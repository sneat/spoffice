using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;
using System.Xml.Linq;
using System.IO;
using SharpSpotLib.Util;

namespace Spoffice.Website.Models.Spotify
{
    public class MetadataApiParser
    {
        private XDocument GetData(string id, string lookupUrl, string folder, string uri)
        {
            string cachePath = HttpContext.Current.Server.MapPath(String.Format("~/{1}{0}{2}{0}{3}{0}{4}.xml", Path.DirectorySeparatorChar, "Cache", "requests", folder, id));
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
       #region artist parsing
        public ArtistOutput ParseArtist(string id)
        {
            return ParseArtist(GetData(id, "http://ws.spotify.com/lookup/1/?uri={0}&extras=albumdetail", "artist", String.Format("{0}:{1}:{2}", "spotify", "artist", id)));
        }
        public ArtistOutput ParseArtist(XDocument artist)
        {
            XElement root = artist.Element(MusicSearch.ns + "artist");
            return ParseArtist(root);
        }
        public ArtistOutput ParseArtist(XElement artist)
        {
            ArtistOutput output = new ArtistOutput();
            output.PublicId = ParseHref(artist);
            output.MusicBrainzId = ParseMusicBrainz(artist);

            XElement xname = artist.Element(MusicSearch.ns + "name");
            if (xname != null)
                output.Name = xname.Value;

            XElement xalbums = artist.Element(MusicSearch.ns + "albums");
            if (xalbums != null)
            {
                var albums = from item in xalbums.Elements(MusicSearch.ns + "album")
                             select ParseAlbum(item);

                output.Albums = albums.ToList();
            }
            return output;
        }
       #endregion

        #region track parsing
        public TrackOutput ParseTrack(string id)
        {
            return ParseTrack(GetData(id, "http://ws.spotify.com/lookup/1/?uri={0}", "track", String.Format("{0}:{1}:{2}", "spotify", "track", id)));
        }
        public TrackOutput ParseTrack(XDocument track)
        {
            return ParseTrack(track.Element(MusicSearch.ns + "track"));
        }
        public TrackOutput ParseTrack(XElement track)
        {
            TrackOutput output = new TrackOutput();
            output.PublicId = ParseHref(track);
            output.MusicBrainzId = ParseMusicBrainz(track);

            XElement xname = track.Element(MusicSearch.ns + "name");
            if (xname != null)
                output.Title = xname.Value;

            XElement xartist = track.Element(MusicSearch.ns + "artist");
            if (xartist != null)
                output.Artist = ParseArtist(xartist);

            XElement xalbum = track.Element(MusicSearch.ns + "album");
            if (xalbum != null)
                output.Album = ParseAlbum(xalbum);

            XElement xlength = track.Element(MusicSearch.ns + "length");
            if (xlength != null)
                output.Length = (int)(Convert.ToDouble(xlength.Value) * 1000);

            return output;
        }
        #endregion

        #region album parsing
        public AlbumOutput ParseAlbum(string id)
        {
            return ParseAlbum(GetData(id, "http://ws.spotify.com/lookup/1/?uri={0}&extras=trackdetail", "album", String.Format("{0}:{1}:{2}", "spotify", "album", id)));
        }
        public AlbumOutput ParseAlbum(XDocument album)
        {
            return ParseAlbum(album.Element(MusicSearch.ns + "album"));
        }
        public AlbumOutput ParseAlbum(XElement album)
        {
            AlbumOutput output = new AlbumOutput();
            output.PublicId = ParseHref(album);
            output.MusicBrainzId = ParseMusicBrainz(album);

            XElement xname = album.Element(MusicSearch.ns + "name");
            if (xname != null)
                output.Name = xname.Value;

            XElement xavailable = album.Element(MusicSearch.ns + "availability");
            if (xavailable != null)
            {
                XElement xterritories = xavailable.Element(MusicSearch.ns + "territories");
                if (xterritories != null)
                    output.IsAvailable = xterritories.Value.Contains("GB");
            }

            XElement xartist = album.Element(MusicSearch.ns + "artist");
            if (xartist != null)
                output.Artist = ParseArtist(xartist);


            XElement xtracks = album.Element(MusicSearch.ns + "tracks");
            if (xtracks != null)
            {
                var tracks = from item in xtracks.Elements(MusicSearch.ns + "track")
                             select ParseTrack(item);

                output.Tracks = tracks.ToList();
            }
            return output;
        }
        #endregion

        protected Guid ParseMusicBrainz(XElement root)
        {
            try
            {
                XElement xmusicbrainz = root.Elements(MusicSearch.ns + "id").Where(x => x.Attribute("type").Value == "mbid").SingleOrDefault();
                if (xmusicbrainz != null)
                    return new Guid(xmusicbrainz.Value);
            }
            catch (Exception e)
            {
            }
            return Guid.Empty;
        }

        protected string ParseHref(XElement root)
        {
            XAttribute xhref = root.Attribute("href");
            if (xhref != null)
                return new SpotifyURI(xhref.Value).Id;

            return null;
        }

        public static ArtistOutput GetArtistById(string id)
        {
            MetadataApiParser parser = new MetadataApiParser();
            return parser.ParseArtist(id);
        }
        public static AlbumOutput GetAlbumById(string id)
        {
            MetadataApiParser parser = new MetadataApiParser();
            return parser.ParseAlbum(id);
        }

        public static TrackOutput GetTrackById(string id)
        {
            MetadataApiParser parser = new MetadataApiParser();
            return parser.ParseTrack(id);
        }
    }
}
