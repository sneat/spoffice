using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models
{
    public interface IRatingRepository
    {
        Rating GetTrackRatingForUser(Track track, User user);
        void DeleteRatingsForTrack(Track track);

        StatusOutput VoteForTrack(Track track, Guid userGuid);
        StatusOutput VoteAgainstTrack(Track track, Guid userGuid);

        StatusOutput VoteForTrack(string trackid, Guid userGuid);
        StatusOutput VoteAgainstTrack(string trackid, Guid userGuid);
    }
}
