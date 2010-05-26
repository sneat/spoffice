using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class Image
    {
        #region fields

        private String _id = null;

        #endregion


        #region properties

        public String Id
        {
            get { return this._id; }
            set
            {
                if (value == null || value.Length != 40 || !Hex.IsHex(value))
                    throw new ArgumentException("Expecting a 40-character hex string.");
                this._id = value;
            }
        }

        public Int32 Height { get; set; }
        public Int32 Width { get; set; }

        #endregion


        #region methods

        #endregion


        #region construction

        public Image()
        {
            this.Width = -1;
            this.Height = -1;
        }

        public Image(String id) : this(id, -1, -1)
        {
        }

        public Image(String id, Int32 width, Int32 height)
        {
            this.Id = id;
            this.Width = width;
            this.Height = height;
        }

        #endregion
    }
}
