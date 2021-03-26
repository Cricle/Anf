using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Kw.Comic
{
    internal static class XComicConst
    {
        public static string DataFolderName = "Datas";

        public static string CacheFolderName = "Caches";

        public static string DbFileName = "comic.db3";

        public static string LocalDataPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

        public static string DataFolderPath= Path.Combine(LocalDataPath, DataFolderName);

        public static string CacheFolderPath = Path.Combine(LocalDataPath,CacheFolderName);

        public static string DbFilePath=Path.Combine(DataFolderPath, DbFileName);

        public static void EnsureDataFolderCreated()
        {
            if (!Directory.Exists(DataFolderPath))
            {
                Directory.CreateDirectory(DataFolderPath);
            }
        }
        public static void EnsureCacheFolderCreated()
        {
            if (!Directory.Exists(CacheFolderPath))
            {
                Directory.CreateDirectory(CacheFolderPath);
            }
        }
    }
}
