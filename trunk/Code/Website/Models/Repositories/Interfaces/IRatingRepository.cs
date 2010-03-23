using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Website.Models
{
    public interface IRatingRepository
    {
        Rating GetTrackRatingForUser(Track track, User user);
        void DeleteRatingsForTrack(Track track);

        bool VoteForTrack(Track track, Guid userGuid);
        bool VoteAgainstTrack(Track track, Guid userGuid);

        bool VoteForTrack(string trackid, Guid userGuid);
        bool VoteAgainstTrack(string trackid, Guid userGuid);
    }
}
