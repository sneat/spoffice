using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Configuration;

namespace Spoffice.Website.Helpers
{
    public class CacheHelper
    {
        private static string _cachepath = null;
        public static string CachePath
        {
            get
            {
                if (_cachepath == null)
                {
                    string path = ConfigurationManager.AppSettings["Spoffice.DataCachePath"];
                    if (String.IsNullOrEmpty(path))
                    {
                        throw new Exception("Spoffice.DataCachePath not set in the web.config");
                    }
                    if (path.StartsWith("~"))
                    {
                        path = path.Replace("~", HttpRuntime.AppDomainAppPath);
                    }
                    _cachepath = path;
                }
                return _cachepath;
            }
        }
        public static void CreateFolder(string path)
        {
            if (!Directory.GetParent(path).Exists)
            {
                CreateFolder(Directory.GetParent(path).FullName);
            }
            if (Path.GetFileName(path) != String.Empty)
            {
                path = new FileInfo(path).Directory.FullName;
            }
            if (!System.IO.Directory.Exists(path))
            {
                System.IO.Directory.CreateDirectory(path);
            }
        }
    }
}
