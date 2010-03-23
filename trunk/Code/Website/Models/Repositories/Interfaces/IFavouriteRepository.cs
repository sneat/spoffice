using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Website.Models
{
    public interface IFavouriteRepository
    {
        List<Favourite> GetUsersFavourites(Guid userid);
        Favourite GetTrackFavouriteForUser(Guid trackid, Guid userid);
        Status AddToFavourites(string trackid, Guid userid);
        Status RemoveFromFavourites(string trackid, Guid userid);
    }
}