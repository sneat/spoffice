using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spoffice.Website.Models.Output;

namespace Spoffice.Website.Services.Music.Browser
{
    public interface IMusicBrowser
    {
        List<ArtistOutput> SearchForArtist(string query);
        List<AlbumOutput> SearchForAlbum(string query);
        List<TrackOutput> SearchForTrack(string query);
        ArtistOutput GetArtistById(Guid id);
        AlbumOutput GetAlbumById(Guid id);
        TrackOutput GetTrackById(Guid id);
    }
}
