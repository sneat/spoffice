using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models.Output
{
    public class FavouritesListOutput
    {
        public FavouritesListOutput(List<Favourite> favourites)
        {
            Favourites = new List<FavouriteOutput>();
            foreach (Favourite favourite in favourites)
            {
                Favourites.Add(favourite.AsOutput());
            }
        }
        public List<FavouriteOutput> Favourites
        {
            get;
            set;
        }
    }
}
