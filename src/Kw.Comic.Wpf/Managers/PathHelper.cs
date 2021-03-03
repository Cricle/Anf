using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Kw.Comic.Wpf.Managers
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
        public static string EnsureName(string name, char invalidRelpaceChar = DefaultInvalidReplaceChar)
        {
            var n = new string(name.Select(x => InvalidChars.Contains(x) ? invalidRelpaceChar : x).ToArray());
            return n;
        }
    }
}
