using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Lib.MusicServices;
using Spoffice.Lib.Interfaces;
using System.Threading;
using Spoffice.Lib.Repositories;
using Spoffice.Lib.MusicPlayers;

namespace Spoffice.Lib
{
    public class Controller
    {
        private static Controller current;
        public static Controller Current
        {
            get
            {
                if (current == null)
                {
                    current = new Controller();
                }
                return current;
            }
        }

        public int maxQueueLength = 2;
        private ITrackRepository repository;
        private IMusicPlayer player;
        public List<Track> queue = new List<Track>();

        #region constructor
        protected Controller()
        {
        }
        #endregion

        #region Init
        public void Init(ITrackRepository _repository)
        {
            Init(_repository, new IrrklangMusicPlayer());
        }
        public void Init(ITrackRepository _repository, IMusicPlayer _player)
        {
            repository = _repository;
            player = _player;
            Thread thread = new Thread(new ThreadStart(processQueue));
            thread.Start();
        }
        #endregion

        private void processQueue()
        {
            while (true)
            {

                // lets fill the queue with new tracks
                if (queue.Count < maxQueueLength)
                {
                    queue.AddRange(repository.GetTracks(maxQueueLength - queue.Count));
                }

                // download the next undownloaded track
                Track trackToDownload = queue.Where(x => x.State == TrackState.Empty).FirstOrDefault();
                if (trackToDownload != null)
                {
                    MusicServiceManager.GetServiceForTrack(trackToDownload).Download(trackToDownload);
                }

                // remove any played and invalid tracks.
                foreach (Track track in queue)
                {
                    if (track.State == TrackState.Played || track.State == TrackState.Invalid)
                    {
                        // if it's invalid we'll let the repository know
                        if (track.State == TrackState.Invalid)
                        {
                            repository.RemoveInvalidTrack(track);
                        }
                        queue.Remove(track);
                    }
                }

                // if the first track on the list isnt playing lets make sure we play it
                Track trackToPlay = queue.FirstOrDefault();
                if (trackToPlay != null && trackToPlay.State == TrackState.Buffered)
                {
                    player.Play(trackToPlay);
                }

                Thread.Sleep(1000);
            }
        }
    }
}
