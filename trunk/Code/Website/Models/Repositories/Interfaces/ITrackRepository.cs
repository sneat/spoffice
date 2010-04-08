using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Website.Models
{
    public interface ITrackRepository
    {
        void AddTrackToPlayHistory(Track track);
        void FullyRemoveTrack(Track track);


        List<Track> GetTracksToPlay(int maxCount);
        List<Track> GetTracksToPlay(List<Guid> exclude);
        List<Track> GetTracksToPlay(int maxCount, List<Guid> exclude);

        Track GetTrackById(Guid id);

        long GetTotalBytesPlayed();
    }
}
