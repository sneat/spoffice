using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models
{
    public class ArtistRepository : IArtistRepository
    {
        #region IArtistRepository Members

        public Artist GetArtistById(Guid artistid)
        {
            return (from m in DataContext.Context.Artists
             where m.Id == artistid
             select m).FirstOrDefault();
        }

        #endregion
    }
}
