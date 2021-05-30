using Anf.Platform.Models.Impl;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Platform.Services.Impl
{
    public class WithImageComicStoreService<TResource,TImage> : ComicStoreService<WithImageComicStoreBox<TResource,TImage>>
    {
        public WithImageComicStoreService(DirectoryInfo folder, int cacheSize = 50) : base(folder, cacheSize)
        {
        }

        protected override WithImageComicStoreBox<TResource,TImage> CreateBox(FileInfo file)
        {
            return new WithImageComicStoreBox<TResource,TImage>(file);
        }
    }
}
