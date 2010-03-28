using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Spoffice.Website.Models.Spotify.MetadataApi;

namespace Spoffice.Website.Models
{
    public partial class Album
    {
        public AlbumNode ConvertToNode()
        {
            AlbumNode node = new AlbumNode();
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
