using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpSpotLib.Media
{
    public class Restriction
    {
        #region properties

        public String Allowed { get; set; }
        public String Forbidden { get; set; }
        public String Catalogues { get; set; }

        #endregion

        #region methods

        public Boolean IsAllowed(String country)
        {
            if (country.Length != 2)
                throw new ArgumentException("Expecting a 2-letter country code.");
            return this.Allowed != null && this.Allowed.ToLower().Contains(country.ToLower());
        }

        public Boolean IsForbidden(String country)
        {
            if (country.Length != 2)
                throw new ArgumentException("Expecting a 2-letter country code.");
            return this.Forbidden != null && this.Forbidden.ToLower().Contains(country.ToLower());
        }

        public Boolean IsCatalogue(String catalogue)
        {
            return this.Catalogues != null && this.Catalogues.ToLower().Contains(catalogue.ToLower());
        }

        #endregion

        #region construction

        public Restriction()
        {
            this.Allowed = null;
            this.Forbidden = null;
            this.Catalogues = null;
        }

        public Restriction(String allowed, String forbidden, String catalogues)
        {
            this.Allowed = allowed;
            this.Forbidden = forbidden;
            this.Catalogues = catalogues;
        }

        #endregion
    }
}
