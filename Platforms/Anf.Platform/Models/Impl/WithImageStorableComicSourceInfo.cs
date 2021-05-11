using Anf.Models;
using Anf.Platform.Services.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Platform.Models.Impl
{
    public class WithImageStorableComicSnapshotInfo<TImage> : ComicSnapshotInfo<WithImageStorableComicSourceInfo<TImage>>
    {
        public WithImageStorableComicSnapshotInfo(ComicSnapshot snapshot) : base(snapshot)
        {
        }

        protected override WithImageStorableComicSourceInfo<TImage> CreateSourceInfo(ComicSnapshot snapshot, ComicSource source, ComicEngine engine)
        {
            var store = AppEngine.GetRequiredService<WithImageComicStoreService<TImage>>();
            var box = store.GetStoreBox(source.TargetUrl);
            return new WithImageStorableComicSourceInfo<TImage>(snapshot, source, engine.GetComicSourceProviderType(source.TargetUrl), box);
        }
    }
}
