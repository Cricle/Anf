using Anf.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Anf.Platform.Models.Impl
{
    public class WithImageStorableComicSourceInfo<TResource,TImage> : StorableComicSourceInfo<WithImageComicStoreBox<TResource,TImage>>
    {
        public WithImageStorableComicSourceInfo(ComicSnapshot snapshot, ComicSource source, IComicSourceCondition condition, WithImageComicStoreBox<TResource,TImage> storeBox) : base(snapshot, source, condition, storeBox)
        {
        }

        protected override WithImageComicStoreBox<TResource,TImage> CreateBox(FileInfo file)
        {
            return new WithImageComicStoreBox<TResource,TImage>(file);
        }
    }
}
