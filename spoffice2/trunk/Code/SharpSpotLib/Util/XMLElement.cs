using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;

namespace SharpSpotLib.Util
{
    public class XMLElement
    {
        #region fields

        private XmlElement _element;

        #endregion


        #region properties

        public XmlElement Element
        {
            get
            {
                return _element;
            }
        }

        public String Text
        {
            get
            {
                return _element.InnerText;
            }
        }

        public String TagName
        {
            get
            {
                return _element.Name;
            }
        }

        #endregion


        #region methods

        public String GetAttribute(String name)
        {
            return _element.HasAttribute(name) ? _element.GetAttribute(name) : null;
        }

        public Boolean HasChild(String name)
        {
            XmlNodeList list = _element.GetElementsByTagName(name);
            foreach (XmlElement node in list)
            {
                if (node.ParentNode == _element)
                    return true;
            }
            return false;
        }

        public XMLElement GetChild(String name)
        {
            XmlNodeList list = _element.GetElementsByTagName(name);
            foreach (XmlElement node in list)
            {
                if (node.ParentNode == _element)
                    return new XMLElement(node);
            }
            return null;
        }

        public String GetChildText(String name)
        {
            XMLElement child = GetChild(name);
            return child != null ? child.Text : null;
        }

        public XMLElement[] GetChildren()
        {
            return GetChildren("*");
        }

        public XMLElement[] GetChildren(String name)
        {
            List<XMLElement> children = new List<XMLElement>();
            XmlNodeList list = _element.GetElementsByTagName(name);
            foreach (XmlElement node in list)
            {
                if (node.ParentNode == _element)
                    children.Add(new XMLElement(node));
            }
            return children.ToArray();
        }

        #endregion


        #region construction

        public XMLElement(XmlElement element)
        {
            _element = element;
        }

        #endregion
    }
}
