using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Services.Music.Downloader;
using Spoffice.Website.Services.Music.Player;
using Spoffice.Website.Models;
using System.Web.Security;

namespace Spoffice.Website.Services.Music
{
    public class MusicService
    {

        #region voting properties
        /// <summary>
        /// The votes for the current track
        /// </summary>
        public List<Guid> Votes = new List<Guid>();

        /// <summary>
        /// The amount of votes for the current track
        /// </summary>
        public int VoteCount
        {
            get
            {
                return Votes.Count;
            }
        }
        public int RequiredVotes
        {
            get
            {
                return (int)Math.Ceiling(((double)Membership.GetNumberOfUsersOnline() / 2));
            }
        }
        #endregion

        #region track list properties
        /// <summary>
        /// The list of upcoming tracks. This should always stay at the length of TrackBufferSize
        /// </summary>
        public List<Track> UpcomingTracks = new List<Track>();

        /// <summary>
        /// The amount of tracks stored in the buffer.
        /// </summary>
        public int TrackBufferSize = 4;

        /// <summary>
        /// Returns the first track (the current) in the upcoming list
        /// </summary>
        public Track CurrentTrack
        {
            get
            {
                return UpcomingTracks.FirstOrDefault();
            }
        }
        #endregion

        /// <summary>
        /// The MusicDownloader used to retrieve the music
        /// </summary>
        public IMusicDownloader Downloader;

        /// <summary>
        /// The music player used to play the music
        /// </summary>
        public IMusicPlayer Player;

        /// <summary>
        /// Track data
        /// </summary>

        public MusicService(IMusicDownloader downloader, IMusicPlayer player)
        {
            Downloader = downloader;
            Player = player;
            OrganizePlaylist();
        }

        /// <summary>
        /// Sorts the play list out
        /// </summary>
        private void OrganizePlaylist()
        {
            System.Diagnostics.Debug.WriteLine("organize playlist");
            // If the current track exists and it's ready to be played
		    if (CurrentTrack != null && CurrentTrack.State == TrackState.Downloaded)
            {

                System.Diagnostics.Debug.WriteLine("Playing CurrentTrack");
                // play it and clear the votes
			    CurrentTrack.Play();
			    Votes.Clear();			
		    }
            // check the length of our upcoming tracks list. If it's not as long as the buffer size..
            int missingTracksCount = TrackBufferSize - UpcomingTracks.Count;
		    if (missingTracksCount > 0){

                System.Diagnostics.Debug.WriteLine("Getting "+missingTracksCount.ToString()+" new tracks");
                // get a load of new tracks
                List<Track> Tracks = DataContext.TrackRepository.GetTracksToPlay(missingTracksCount);
                if (Tracks.Count < missingTracksCount)
                {

                }
			    // assign them the events and add them to our list
                foreach (Track track in Tracks){				
				    track.Player = Player;
				    track.Downloader = Downloader;
				    track.OnTrackDownloaded += new Track.TrackDownloadedDlg(OnTrackDownloaded);
                    track.OnTrackDownloadError += new Track.TrackDownloadErrorDlg(OnTrackDownloadError);
                    track.OnTrackPlayed += new Track.TrackPlayedDlg(OnTrackPlayed);
				    UpcomingTracks.Add(track);
			    }
		    }
            DownloadNextTrack();
        }

        public void DownloadNextTrack()
        {
            Track previous = null;
            foreach (Track track in UpcomingTracks)
            {
                if (track.State == TrackState.New && (previous == null || previous.State != TrackState.Downloading))
                {

                    System.Diagnostics.Debug.WriteLine("Downloading next track");
                    track.Download();
                    break;
                }
                previous = track;
            }    
        }

        private void OnTrackDownloaded(Track track)
        {
            System.Diagnostics.Debug.WriteLine("track downloaded!");
            OrganizePlaylist();
        }
        private void OnTrackPlayed(Track track)
        {
            System.Diagnostics.Debug.WriteLine("OnTrackPlayed");
            DataContext.TrackRepository.AddTrackToPlayHistory(track);
            // remove played delegate?
            UpcomingTracks.Remove(track);
            OrganizePlaylist();
        }
        private void OnTrackDownloadError(Track track)
        {
            System.Diagnostics.Debug.WriteLine("OnTrackDownloadError");
            DataContext.TrackRepository.FullyRemoveTrack(track);
            UpcomingTracks.Remove(track);
            OrganizePlaylist();
        }

        #region voting

        public bool AddVote(Guid userId)
        {
            if (!Votes.Contains(userId))
            {
                Votes.Add(userId);
                if (VoteCount >= RequiredVotes)
                {
                    CurrentTrack.Stop();
                }
                return true;
            }
            return false;
        }
        #endregion

    }
}
