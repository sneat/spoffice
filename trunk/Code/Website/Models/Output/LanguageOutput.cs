using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.IO;
using System.Text.RegularExpressions;

namespace Spoffice.Website.Models.Output
{
    [Serializable]
    public class LanguageOutput
    {
        
        public LanguageOutput(string lang)
        {
            Language = new Dictionary<string, string>();
            CultureInfo culture = null;
            if (String.IsNullOrEmpty(lang))
            {
                culture = CultureInfo.CurrentCulture;
            }
            else
            {
                culture = new CultureInfo(lang);
            }

            ResourceManager manager = new ResourceManager("Spoffice.Website.App_GlobalResources.Strings", Assembly.GetExecutingAssembly());
            
            ResourceSet resources = manager.GetResourceSet(culture, true, true);
            
            IDictionaryEnumerator enumerator = resources.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Language.Add((string)enumerator.Key, (string)enumerator.Value);
            }

            AvailableLanguages = new List<string>();
            string[] list = Directory.GetFiles(HttpContext.Current.Server.MapPath("App_GocalResources"));
            foreach (string str in list)
            {
                Regex regex = new Regex("[a-zA-Z-]*.resx");
                string language = regex.Match(str).ToString().Replace(".resx", String.Empty);

                if (language == "aspx")
                {
                    language = CultureInfo.CurrentCulture.NativeName;
                }
                else
                {
                    language = CultureInfo.GetCultureInfo(language).NativeName;
                }
            }
        }
        public List<string> AvailableLanguages
        {
            get;
            set;
        }
        public Dictionary<string, string> Language
        {
            get;
            set;
        }
    }
}
