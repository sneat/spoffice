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
using System.Configuration;

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
                culture = CultureInfo.CreateSpecificCulture(lang);
                System.Threading.Thread.CurrentThread.CurrentCulture = culture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
            }


            ResourceManager manager = new ResourceManager("Spoffice.Website.App_GlobalResources.Strings", Assembly.GetExecutingAssembly());
            
            ResourceSet resources = manager.GetResourceSet(culture, true, true);
            
            IDictionaryEnumerator enumerator = resources.GetEnumerator();

            while (enumerator.MoveNext())
            {
                Language.Add((string)enumerator.Key, (string)enumerator.Value);
            }

            AvailableLanguages = new Dictionary<string, string>();
            string[] languageCodes = ConfigurationManager.AppSettings["Spoffice.AvailableLanguages"].Split(new char[] { ',' });
            foreach (string str in languageCodes)
            {
                CultureInfo info = CultureInfo.GetCultureInfo(str);
                AvailableLanguages.Add(info.TwoLetterISOLanguageName, info.DisplayName);
            }
            CurrentCulture = CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
        }
        public string CurrentCulture
        {
            get;
            set;
        }
        public Dictionary<string, string> AvailableLanguages
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
