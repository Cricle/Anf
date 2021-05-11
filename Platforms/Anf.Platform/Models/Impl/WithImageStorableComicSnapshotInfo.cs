using Anf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Platform.Models.Impl
{
    public class WithImageStorableComicSourceInfo<TImage>: StorableComicSourceInfo<WithImageComicStoreBox<TImage>>
    {
        public WithImageStorableComicSourceInfo(ComicSnapshot snapshot, ComicSource source, IComicSourceCondition condition, WithImageComicStoreBox<TImage> storeBox) : base(snapshot, source, condition, storeBox)
        {
        }

        protected override WithImageComicStoreBox<TImage> CreateBox(FileInfo file)
        {
            return new WithImageComicStoreBox<TImage>(file);
        }
    }
}
