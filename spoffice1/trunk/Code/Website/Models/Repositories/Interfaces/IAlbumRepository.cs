using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Website.Models
{
    public interface IAlbumRepository
    {
        Album GetAlbumById(Guid albumid);
    }
}
