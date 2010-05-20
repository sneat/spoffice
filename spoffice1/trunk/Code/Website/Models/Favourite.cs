using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models
{
    public partial class Favourite
    {
        public FavouriteOutput AsOutput()
        {
            FavouriteOutput node = new FavouriteOutput();
            if (Track != null)
                node.Track = Track.AsOutput();

            if (User != null)
                node.UserId = User.UserId;
            return node;
        }
    }
}
