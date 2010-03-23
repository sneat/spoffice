using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class File
    {
        #region fields

        private String _id = null;

        #endregion

        #region properties

        public String Id
        {
            get
            {
                return _id;
            }
            set
            {
                if (value == null || value.Length != 40 || !Hex.IsHex(value))
                    throw new ArgumentException("Expecting a 40-character hex string.");
                _id = value;
            }
        }

        public String Format { get; set; }

        #endregion

        #region construction

        protected File()
        {
            this.Format = null;
        }

        public File(String id, String format)
        {
            this.Id = id;
            this.Format = format;
        }

        #endregion
    }
}
