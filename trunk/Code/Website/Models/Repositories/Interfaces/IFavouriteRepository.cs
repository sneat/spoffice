using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models
{
    public interface IFavouriteRepository
    {
        List<Favourite> GetUsersFavourites(Guid userid);
        Favourite GetTrackFavouriteForUser(Guid trackid, Guid userid);
        StatusOutput AddToFavourites(string trackid, Guid userid);
        StatusOutput RemoveFromFavourites(string trackid, Guid userid);
    }
}