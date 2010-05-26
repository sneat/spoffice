using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Media
{
    public class Disc
    {
        #region properties

        public Int32 Number { get; set; }
        public String Name { get; set; }
        public List<Track> Tracks { get; set; }

        #endregion

        #region construction

        public Disc()
        {
            this.Number = -1;
            this.Name = null;
            this.Tracks = new List<Track>();
        }

        public Disc(Int32 number, String name)
        {
            this.Number = number;
            this.Name = name;
            this.Tracks = new List<Track>();
        }

        #endregion
    }
}
