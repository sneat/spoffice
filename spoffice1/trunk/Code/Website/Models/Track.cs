﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Services;
using Spoffice.Website.Models.Output;
using System.Configuration;
namespace Spoffice.Website.Models
{
    public enum TrackState
    {
        New,
        Downloading,
        Downloaded,
        Failed,
        Playing,
        Played
    }
    public partial class Track
    {

        public IMusicPlayer Player;
        public IMusicDownloader Downloader;

        public long BytesDownloaded;
        public long BytesTotal;
        public int Progress = 0;

        /// <summary>
        /// On track download
        /// </summary>
        /// <param name="track"></param>
        public delegate void TrackDownloadedDlg(Track track);
        public event TrackDownloadedDlg OnTrackDownloaded;

        /// <summary>
        /// On track played
        /// </summary>
        /// <param name="track"></param>
        public delegate void TrackPlayedDlg(Track track);
        public event TrackPlayedDlg OnTrackPlayed;

        /// <summary>
        /// On track download error
        /// </summary>
        /// <param name="track"></param>
        public delegate void TrackDownloadErrorDlg(Track track);
        public event TrackDownloadErrorDlg OnTrackDownloadError;


        /// <summary>
        /// The current state of the track
        /// </summary>
        public TrackState State = TrackState.New;

        /// <summary>
        /// Path to the music cache on the harddrive
        /// </summary>
        public string FilePath
        {
            get;
            set;
        }

        /// <summary>
        /// Play the track using the IMusicPlayer
        /// </summary>
        public void Play()
        {
            State = TrackState.Playing;
            Player.PlayTrack(this);
        }

        /// <summary>
        /// Stop playing the track
        /// </summary>
        public void Stop()
        {
            if (Player != null)
            {
                Player.Stop();
            }
        }

        /// <summary>
        /// When a track is played, fire the OnPlayed event
        /// </summary>
        public void OnPlayed()
        {
            State = TrackState.Played;
            OnTrackPlayed(this);
        }

        /// <summary>
        /// Download the track using the IMusicDownloader
        /// </summary>
        public void Download()
        {
            State = TrackState.Downloading;
            Downloader.DownloadTrack(this);
        }

        /// <summary>
        /// When a track is downloaded, fire the OnTrackDownloaded event
        /// </summary>
        public void OnDownloaded()
        {
            State = TrackState.Downloaded;
            OnTrackDownloaded(this);
        }

        /// <summary>
        /// When a track has a download error, fire the OnDownloadError event
        /// </summary>
        public void OnDownloadError()
        {
            State = TrackState.Failed;
            OnTrackDownloadError(this);
        }
        public TrackOutput AsOutput()
        {
            TrackOutput node = new TrackOutput();
            node.Length = Length;
            if (MusicBrainzId != null){
                node.MusicBrainzId = (Guid)MusicBrainzId;
            }
            node.State = State;
            node.Id = Id;
            node.Title = Title;
            if (Album != null)
            {
                node.Album = Album.AsOutput();
            }
            if (Artist != null)
            {
                node.Artist = Artist.AsOutput();
            }
            return node;
        }
    }
}
