using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models.Output
{
    public class FavouriteOutput
    {
        public Guid UserId
        {
            get;
            set;
        }
        public TrackOutput Track
        {
            get;
            set;
        }
    }
}
