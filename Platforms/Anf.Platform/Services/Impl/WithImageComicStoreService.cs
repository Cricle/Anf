using Anf.Platform.Models.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Platform.Services.Impl
{
    public class WithImageComicStoreService<TImage> : ComicStoreService<WithImageComicStoreBox<TImage>>
    {
        public WithImageComicStoreService(DirectoryInfo folder, int cacheSize = 50) : base(folder, cacheSize)
        {
        }

        protected override WithImageComicStoreBox<TImage> CreateBox(FileInfo file)
        {
            return new WithImageComicStoreBox<TImage>(file);
        }
    }
}
