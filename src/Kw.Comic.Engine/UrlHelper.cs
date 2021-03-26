using System;
using System.Collections.Generic;
using System.Text;

namespace Kw.Comic.Engine
{
    public static class UrlHelper
    {
        public static bool IsWebsite(this string str)
        {
            return str.StartsWith("http://") ||
                str.StartsWith("https://") ||
                str.StartsWith("www.");
        }
        public static string GetUrl(this string str)
        {
            if (str.StartsWith("www."))
            {
                return string.Concat("http://", str);
            }
            return str;
        }
    }
}
