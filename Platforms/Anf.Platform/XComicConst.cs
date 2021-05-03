using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf
{
    public static class XComicConst
    {
        public static string DataFolderName = "Datas";

        public static string CacheFolderName = "Caches";

        public static string BookFolderName = "Books";

        public static string StoreFolderName = "Stores";

        public static string SettingFileName = "anf.json";
#if NETSTANDARD1_4
        public static string SettingFileFolder = Path.Combine(Directory.GetCurrentDirectory(), SettingFileName);
#else
        public static string SettingFileFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,SettingFileName);
#endif
    }
}
