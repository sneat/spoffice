using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models
{
    public class FavouriteRepository : IFavouriteRepository
    {

        #region IFavouriteRepository Members

        public List<Favourite> GetUsersFavourites(Guid userid)
        {
            return DataContext.Context.Favourites.Include("Track.Artist").Where(f => f.User.UserId == userid).ToList();
        }

        #endregion
    }
}
