using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Media
{
    public class Biography
    {
        #region properties

        public String Text { get; set; }
        public List<Image> Portraits { get; set; }

        #endregion

        #region construction

        public Biography()
        {
            this.Text = null;
            this.Portraits = new List<Image>();
        }

        public Biography(String text)
        {
            this.Text = text;
            this.Portraits = new List<Image>();
        }

        #endregion
    }
}
