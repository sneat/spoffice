﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SharpSpotLib;
using System.Threading;
using System.IO;
using Spoffice.Website.Models;
using Spoffice.Website.Models.Output;
using System.Configuration;
using Spoffice.Website.Services.Music.CoverGrabber;

namespace Spoffice.Website.Services.Music.Downloader
{
  
    public class SpotifyDownloader : IMusicDownloader, ICoverGrabber
    {
        private SharpSpotConnection connection;
        public SpotifyDownloader()
        {
            string username = ConfigurationManager.AppSettings["Spotify.Username"];
            string password = ConfigurationManager.AppSettings["Spotify.Password"];
            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                throw new Exception("Please specify Spotify.Username and Spotify.Password in the web.config");
            }
            connection = new SharpSpotConnection(new SharpSpotLib.Cache.FileCache(), new TimeSpan(0, 0, 20));
            try
            {
                connection.Login(username, password);
            }
            catch (Exception e)
            {
                throw e; // throw a better error please!
            }
            Thread background = new Thread(new ThreadStart(connection.Run));
            background.IsBackground = true;
            background.Start();
        }

        #region IMusicDownloader Members

        /// <summary>
        /// Download the track
        /// </summary>
        /// <param name="track"></param>
        public void DownloadTrack(Track track)
        {
            if (System.IO.File.Exists(track.CachePath))
            {
                track.BytesTotal = new FileInfo(track.CachePath).Length;
                track.BytesDownloaded = track.BytesTotal;
                track.OnDownloaded();
            }
            else
            {
                // download the track
                Thread thread = new Thread(new ParameterizedThreadStart(DoDownload));
                thread.Start(track);
            }
        }

        /// <summary>
        /// Do the download. Call this as a thread!
        /// </summary>
        /// <param name="t"></param>
        private void DoDownload(object t)
        {
            Models.Track track = (Models.Track)t;
            SharpSpotLib.Media.MusicStream stream = null;
            bool success = false;
            try
            {
                // browse for the track id in spotify
                SharpSpotLib.Media.Result result = connection.BrowseTrack(track.Id.ToString("N"));

                // find the first track result
                SharpSpotLib.Media.Track spotifytrack = result.Tracks.First();

                // find the last file for that track
                stream = connection.GetMusicStream(spotifytrack, spotifytrack.Files.Last());

                track.BytesTotal = stream.Length;

                // while we've got bytes to get lets sleep
                while (stream.AvailableLength < stream.Length || stream.Length == 0)
                {
                    track.BytesDownloaded = stream.AvailableLength;
                    System.Threading.Thread.Sleep(500);
                }

                track.BytesDownloaded = track.BytesTotal;
                // write all the bytes to the cache file
                System.IO.File.WriteAllBytes(track.CachePath, stream.GetBuffer());
                success = true;

            }
            catch (Exception ex)
            {
                track.OnDownloadError();
                success = false;
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                    stream = null;
                }
            }
            if (success)
                track.OnDownloaded();
        }

        #endregion

        #region ICoverGrabber Members

        public string GetCoverPath(AlbumOutput album)
        {
            if (File.Exists(album.AlbumartCachePath))
            {
                return album.AlbumartCacheWebPath;
            }
            else
            {
                try
                {
                    SharpSpotLib.Media.Album spotify_album = connection.BrowseAlbum(album.Id.ToString("N"));
                    if (spotify_album != null && spotify_album.Cover != null)
                    {
                        System.Drawing.Image img = connection.Image(spotify_album.Cover);
                        if (img != null)
                        {
                            img.Save(album.AlbumartCachePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                            return album.AlbumartCacheWebPath;
                        }
                    }
                }
                catch (Exception ex)
                {
                    //
                }
            }
            return null;
        }

        #endregion

    
    }
}
