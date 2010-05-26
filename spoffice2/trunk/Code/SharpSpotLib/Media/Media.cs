using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class Media
    {
        #region fields

        private String _id = null;
        protected List<String> _redirects;
        private Single _popularity;
        private List<Restriction> _restrictions;
        private Dictionary<String, String> _externalIds;

        #endregion

        #region properties

        public String Id
        {
            get { return this._id; }
            set
            {
                if (value == null || value.Length != 32 || !Hex.IsHex(value))
                    throw new ArgumentException("Expecting a 32-character hex string.");
                this._id = value;
            }
        }

        public String[] Redirects
        {
            get
            {
                return _redirects.ToArray();
            }
        }

        public Single Popularity 
        { 
            get 
            { 
                return _popularity; 
            }
            set
            {
                if (value != Single.NaN && (value < 0.0 || value > 1.0))
                    throw new ArgumentException("Expecting a value from 0.0 to 1.0 or Float.NAN.");
                this._popularity = value;
            }
        }

        public List<Restriction> Restrictions
        {
            get { return _restrictions; }
            set { _restrictions = value; }
        }

        public Dictionary<String, String> ExternalIds
        {
            get { return _externalIds; }
            set { _externalIds = value; }
        }


        #endregion

        #region methods

        public void AddRedirect(String redirect)
        {
            _redirects.Add(redirect);
        }

        public Boolean IsRestricted(String country, String catalogue)
        {
		    if(country.Length != 2)
			    throw new ArgumentException("Expecting a 2-letter country code.");
		    
    		foreach (Restriction restriction in this._restrictions)
            {
			    if(restriction.IsCatalogue(catalogue) && (restriction.IsForbidden(country) || !restriction.IsAllowed(country)))
				    return true;
		    }
		    return false;
	    }

        public String GetExternalId(String service)
        {
            if (_externalIds.ContainsKey(service))
                return _externalIds[service];
            return null;
        }

        public static Media FromXMLElement(XMLElement mediaElement)
        {
            Media media = new Media();

            if (mediaElement.HasChild("id"))
                media.Id = mediaElement.GetChildText("id");

            if (mediaElement.HasChild("popularity"))
            {
                Single result = Single.NaN;
                if (SingleUtilities.TryParse(mediaElement.GetChildText("popularity"), out result))
                    media.Popularity = result;
            }

            /* OLD:
            var idChildren = mediaElement.GetElementsByTagName("id");
            if (idChildren.Count > 0)
                media.Id = idChildren[0].InnerText;

            var popularityChildren = mediaElement.GetElementsByTagName("popularity");
            if (popularityChildren.Count > 0)
            {
                Single result = Single.NaN;
                if (Single.TryParse(popularityChildren[0].InnerText, out result))
                    media.Popularity = result;
            }*/

            return media;
        }

        #endregion

        #region construction

        protected Media()
        {
            this._redirects = new List<String>();
            this._popularity = Single.NaN;
            this._restrictions = new List<Restriction>();
            this._externalIds = new Dictionary<String, String>();
        }

        protected Media(String id) : this()
        {
            this.Id = id;
        }

        #endregion
    }
}
