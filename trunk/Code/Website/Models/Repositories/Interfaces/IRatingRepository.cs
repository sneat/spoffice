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

        Status VoteForTrack(Track track, Guid userGuid);
        Status VoteAgainstTrack(Track track, Guid userGuid);

        Status VoteForTrack(string trackid, Guid userGuid);
        Status VoteAgainstTrack(string trackid, Guid userGuid);
    }
}
