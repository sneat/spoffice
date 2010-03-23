using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Spoffice.Website.Helpers
{
    public class StringFormatter
    {
        private static readonly long kilobyte = 1024;
        private static readonly long megabyte = 1024 * kilobyte;
        private static readonly long gigabyte = 1024 * megabyte;
        private static readonly long terabyte = 1024 * gigabyte;
        public static string MillisecondsToString(int milliseconds)
        {
            TimeSpan ts = new TimeSpan(0, 0, 0, 0, milliseconds);
            int Hours = (int)ts.TotalHours;
            string str = String.Format("{0}s", ts.Seconds);
            if (ts.Minutes > 0) str = String.Format("{0}m {1}", ts.Minutes, str);
            if (Hours > 0) str = String.Format("{0}h {1}", Hours, str);
            return str;
        }

        public static string BytesToString(long size)
        {
            double s = size;
            string[] format = new string[] { "{0} bytes", "{0} KB", "{0} MB", "{0} GB", "{0} TB", "{0} PB", "{0} EB", "{0} ZB", "{0} YB" };
            int i = 0;
            while (i < format.Length - 1 && s >= 1024)
            {
                s = (100 * s / 1024) / 100.0;
                i++;
            }
            return string.Format(format[i], s.ToString("###,###,##0.##")); 
        }
    }
}
