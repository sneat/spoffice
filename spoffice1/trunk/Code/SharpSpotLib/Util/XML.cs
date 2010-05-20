using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.IO;

namespace SharpSpotLib.Util
{
    internal static class XML
    {
	    public static XMLElement Load(FileStream xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.Load(xml);
                return new XMLElement(doc.DocumentElement);
            }
            catch (Exception)
            {
                return null;
            }
	    }
    	
	    public static XMLElement Load(String xml)
        {
            try
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(xml);
                return new XMLElement(doc.DocumentElement);
            }
            catch (Exception)
            {
                return null;
            }
	    }
    	
	    public static XMLElement Load(Byte[] xml, Encoding charset)
        {
            return Load(charset.GetString(xml, 0, xml.Length));
	    }
    }
}
