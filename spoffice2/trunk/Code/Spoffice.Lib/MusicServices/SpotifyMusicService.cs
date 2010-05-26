using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.Interfaces;
using System.Threading;
using SharpSpotLib;
using SharpSpotLib.Media;
using SharpSpotLib.Util;

namespace Spoffice.Lib.MusicServices
{
    public class SpotifyMusicService : IMusicService
    {
        private SharpSpotConnection connection;
        public SpotifyMusicService()
        {
            connection = new SharpSpotConnection(new SharpSpotLib.Cache.FileCache(), new TimeSpan(0, 0, 20));
            connection.Login("Mikeemoo", "");
            Thread background = new Thread(new ThreadStart(connection.Run));
            background.IsBackground = true;
            background.Start();
        }
        #region IMusicService Members
        public void Download(Track track)
        {
            track.State = TrackState.Buffering;
            Thread thread = new Thread(new ParameterizedThreadStart(doDownload));
            thread.Start(track);
        }

        #endregion

        private void doDownload(object otrack)
        {
            Track track = otrack as Track;
            track.FilePath = String.Format("c:\\temp\\{0}.ogg", track.Id.ToString());
            MusicStream stream = null;
            try
            {
                Result result = connection.BrowseTrack(track.Id.ToString("N"));
                SharpSpotLib.Media.Track spotifytrack = result.Tracks.First();
                System.Diagnostics.Debug.WriteLine(SpotifyURI.ToBase62(spotifytrack.Id));
                File file = spotifytrack.Files.LastOrDefault();
                if (file == null)
                {
                    track.State = TrackState.Invalid;
                    return;
                }
                stream = connection.GetMusicStream(spotifytrack, file);
                DateTime startTime = DateTime.Now;
                while ((stream.AvailableLength < stream.Length || stream.Length == 0))
                {
                    if (DateTime.Now.AddSeconds(-15) > startTime && stream.AvailableLength == 0)
                    {
                        if (spotifytrack.Files.Count > 0)
                        {
                            spotifytrack.Files.Remove(file);
                            file = spotifytrack.Files.LastOrDefault();
                            if (file == null)
                            {
                                track.State = TrackState.Invalid;
                                return;
                            }
                            startTime = DateTime.Now;
                            stream = connection.GetMusicStream(spotifytrack, file); 
                        }
                    }
                    Thread.Sleep(500);
                }
                System.IO.File.WriteAllBytes(track.FilePath, stream.GetBuffer());
            }
            catch (Exception e)
            {
                track.State = TrackState.Invalid;
                return;
            }
            finally
            {
                stream = null;
            }
            track.State = TrackState.Buffered;
            
        }
    }
}
