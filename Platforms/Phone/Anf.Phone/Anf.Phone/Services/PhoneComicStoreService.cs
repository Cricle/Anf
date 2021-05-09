using Anf.Phone.Models;
using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Phone.Services
{
    public class PhoneComicStoreService: ComicStoreService<PhoneComicStoreBox>
    {
        public PhoneComicStoreService(DirectoryInfo folder, int cacheSize = 50) : base(folder, cacheSize)
        {
        }

        protected override PhoneComicStoreBox CreateBox(FileInfo file)
        {
            return new PhoneComicStoreBox(file);
        }
    }
}
