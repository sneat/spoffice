using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using SharpSpotLib.Exceptions;
using SharpSpotLib.Enums;
using SharpSpotLib.Util;

namespace SharpSpotLib.Util
{
    public class SpotifyURI
    {
        #region fields

        private SpotifyURIType _type;
        private String _id;

        #endregion

        #region properties

        public SpotifyURIType Type
        {
            get { return _type; }
        }

        public String Id
        {
            get { return _id; }
        }

        public Boolean IsAlbumURI { get { return Type == SpotifyURIType.ALBUM; } }
        public Boolean IsArtistURI { get { return Type == SpotifyURIType.ARTIST; } }
        public Boolean IsTrackURI { get { return Type == SpotifyURIType.TRACK; } }

        #endregion

        #region methods

        public override string ToString()
        {
            return String.Format("spotify:{0}:{1}", EnumUtil.GetName(typeof(SpotifyURIType), this.Type).ToLower(), this.Id);
        }

        public static String ToHex(String base62)
        {
            String hex = BaseConvert.Convert(base62, 62, 16);
            if (hex.Length >= 32)
                return hex;
            else
                return new String('0', 32 - hex.Length) + hex;
        }

        public static String ToBase62(String hex)
        {
            String base62 = BaseConvert.Convert(hex, 16, 62);
            if (base62.Length >= 22)
                return base62;
            else
                return new String('0', 22 - base62.Length) + base62;
        }

        #endregion

        #region construction

        public SpotifyURI(String uri)
        {
            try
            {
                Match regexpMatch = Regex.Match(uri, "spotify:(artist|album|track):([0-9A-Za-z]{22})");
                if (regexpMatch.Success)
                {
                    String type = regexpMatch.Groups[1].Value;
                    this._type = (SpotifyURIType)Enum.Parse(typeof(SpotifyURIType), type, true);
                    this._id = regexpMatch.Groups[2].Value;
                }
                else
                    throw new InvalidSpotifyURIException();
            }
            catch (Exception ex)
            {
                throw new InvalidSpotifyURIException(ex);
            }
        }

        #endregion
    }
}
