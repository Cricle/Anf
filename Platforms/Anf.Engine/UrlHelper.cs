using System;
using System.Collections.Generic;
using System.Text;

namespace Anf
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
        public static string FastGetHost(string address)
        {
            var len = address.Length;
            char c;
            int start = 0;
            int end = len;
            for (int i = 0; i < len; i++)
            {
                c = address[i];
                if ((c == '/') || (c == '?'))
                {
                    end = i;
                    break;
                }
                else if ((c == ':') && (len - i) > 3 && (address[i + 1] == '/') && (address[i + 2] == '/'))
                {
                    start = i + 3;
                    i = start;
                }
            }
            return address.Substring(start, end - start);
        }

    }
}
