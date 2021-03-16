using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Essentials;

namespace Kw.Comic.Essentials.Stores
{
    public class ComicStoreManager
    {
        public static readonly string AppPath = FileSystem.AppDataDirectory;

        public string BasePath { get; }

        public void Store()
        {

        }
    }
}
