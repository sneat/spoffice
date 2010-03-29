using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Models
{
    public partial class Artist
    {
        public ArtistOutput AsOutput()
        {
            ArtistOutput node = new ArtistOutput();
            if (MusicBrainzId != null)
            {
                node.MusicBrainzId = (Guid)MusicBrainzId;
            }
            node.PrivateId = Id;
            node.Name = Name;
            
            return node;
        }
    }
}
