using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Kw.Comic.Engine.Easy
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
        public unsafe static string EnsureName(string name,char invalidChar=DefaultInvalidReplaceChar)
        {
            int i = 0;
            return string.Create(name.Length, name, (x, y) =>
            {
                var c = y[i];
                if (c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c <= 'Z')
                {
                    x[i] = c;
                }
                else if (InvalidChars.Contains(c))
                {
                    x[i] = invalidChar;
                }
                else
                {
                    x[i] = c;
                }
                i++;
            });
        }
#else
        public unsafe static string EnsureName(string name,char invalidChar=DefaultInvalidReplaceChar)
        {
            char* arr = stackalloc char[name.Length];
            var len = name.Length;
            fixed (char* ptr = name)
            {
                char c;
                for (int i = 0; i < len; i++)
                {
                    c = ptr[i];
                    if (c >= '0' && c <= '9' || c >= 'a' && c <= 'z' || c >= 'A' && c < 'Z')
                    {
                        *(arr + i) = c;
                    }
                    else if (InvalidChars.Contains(c))
                    {
                        *(arr + i) = invalidChar;
                    }
                    else
                    {
                        *(arr + i) = c;
                    }
                }
            }
            return new string(arr);
        }

#endif
    }
}
