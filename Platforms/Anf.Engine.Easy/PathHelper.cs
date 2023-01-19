using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Anf.Easy
{
    public static class PathHelper
    {
        public const char DefaultInvalidReplaceChar = '_';

        private static readonly HashSet<char> InvalidChars = new HashSet<char>(Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsInvalidChar(char c)
        {
            return InvalidChars.Contains(c);
        }
        public static void EnsureCreated(string name)
        {
            if (!Directory.Exists(name))
            {
                Directory.CreateDirectory(name);
            }
        }
#if NETSTANDARD2_1
        public static string EnsureName(string name,char invalidChar=DefaultInvalidReplaceChar)
        {
            return string.Create(name.Length, name, (x, y) =>
            {
                char c;
                for (int i = 0; i < x.Length; i++)
                {
                    c = y[i];

                    if (c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z'||!InvalidChars.Contains(c))
                    {
                        x[i] = c;
                    }
                    else
                    {
                        x[i] = invalidChar;
                    }
                }
            });
        }
#else
        public static string EnsureName(string name, char invalidChar = DefaultInvalidReplaceChar)
        {
            var len = name.Length;
            var arr = Encoding.UTF8.GetBytes(name);
            byte c;
            for (int i = 0; i < len; i++)
            {
                c = arr[i];
                if (InvalidChars.Contains((char)c))
                {
                    arr[i] = (byte)invalidChar;
                }
            }
            return Encoding.UTF8.GetString(arr);
        }
#endif
    }
}
