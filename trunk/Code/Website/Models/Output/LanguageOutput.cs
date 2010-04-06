using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Resources;
using System.Collections;
using System.Globalization;
using System.Reflection;

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
        }
        public Dictionary<string, string> Language
        {
            get;
            set;
        }
    }
}
