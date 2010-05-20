using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.SharpZipLib.Checksums;
using SharpSpotLib.Media;

namespace SharpSpotLib.Util
{
    internal class SpotifyChecksum
    {
        #region fields

        private Adler32 _addler;

        #endregion


        #region properties

        public Int64 ChecksumValue
        {
            get
            {
                return _addler.Value;
            }
        }

        #endregion


        #region methods

        private void Update(Byte[] data)
        {
            _addler.Update(data);
        }

        private void Update(Byte data)
        {
            _addler.Update(data);
        }

        public void Update(Playlist playlist)
        {
            this.Update(Hex.ToBytes(playlist.Id));
            this.Update((Byte)0x02);
        }

        public void Update(Artist artist)
        {
            this.Update(Hex.ToBytes(artist.Id));
            this.Update((Byte)0x02); // TODO: is it really 0x02?
        }

        public void Update(Album album)
        {
            this.Update(Hex.ToBytes(album.Id));
            this.Update((Byte)0x02); // TODO: is it really 0x02?
        }

        public void Update(Track track)
        {
            this.Update(Hex.ToBytes(track.Id));
            this.Update((Byte)0x01);
        }

        #endregion


        #region construction

        public SpotifyChecksum()
        {
            _addler = new Adler32();
        }

        #endregion
    }
}
