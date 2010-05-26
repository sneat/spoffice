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

        public int maxQueueLength = 4;
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
            Init(_repository, new WMPMusicPlayer());
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
                if (queue.Where(x => x.State == TrackState.Buffering).Count() == 0)
                {
                    Track trackToDownload = queue.Where(x => x.State == TrackState.Empty).FirstOrDefault();
                    if (trackToDownload != null)
                    {
                        MusicServiceManager.DownloadTrack(trackToDownload);
                    }
                }

                foreach (Track invalidTrack in queue.Where(x => x.State == TrackState.Invalid))
                {
                    repository.RemoveInvalidTrack(invalidTrack);
                }
                queue.RemoveAll(x => x.State == TrackState.Played || x.State == TrackState.Invalid);

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
