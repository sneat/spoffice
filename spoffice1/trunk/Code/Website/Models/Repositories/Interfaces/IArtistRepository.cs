using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spoffice.Website.Models
{
    public interface IArtistRepository
    {
        Artist GetArtistById(Guid artistid);
    }
}
