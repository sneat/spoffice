using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Models
{
    public class AlbumRepository : IAlbumRepository
    {
        #region IAlbumRepository Members

        public Album GetAlbumById(Guid albumid)
        {
            return (from m in DataContext.Context.Albums
                    where m.Id == albumid
                           select m).FirstOrDefault();
        }

        #endregion
    }
}
