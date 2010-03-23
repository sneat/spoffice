using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpSpotLib.Util;

namespace SharpSpotLib.Media
{
    public class User
    {
        #region properties

        public String Name { get; set; }
        public String Country { get; set; }
        public String Type { get; set; }
        public String Notification { get; set; }

        public Boolean IsPremium
        {
            get
            {
                return this.Type == "premium";
            }
        }

        #endregion


        #region methods

        public override string ToString()
        {
            return String.Format("[User: {0}, {1}, {2}]", this.Name, this.Country, this.Type);
        }

        public static User FromXMLElement(XMLElement prodinfoElement, User user)
        {
            //Old
            /*XmlElement productElement = prodinfoElement["product"];
            user.Type = productElement["type"].InnerText;*/

            XMLElement productElement = prodinfoElement.GetChild("product");
            user.Type = productElement.GetChildText("type");
            return user;
        }

        #endregion


        #region construction

        public User(String name)
            : this(name, null, null)
        {
        }

        public User(String name, String country, String type)
        {
            this.Name = name;
            this.Country = country;
            this.Type = type;
            this.Notification = null;
        }

        #endregion
    }
}
