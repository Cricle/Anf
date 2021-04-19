﻿using Anf.Avalon.Services;
using Anf.Models;
using Anf.Platform.Models;
using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Avalon.Models
{
    public class AvalonStorableComicSourceInfo : StorableComicSourceInfo<AvalonComicStoreBox>
    {
        public AvalonStorableComicSourceInfo(ComicSnapshot snapshot, ComicSource source, IComicSourceCondition condition, AvalonComicStoreBox storeBox) : base(snapshot, source, condition, storeBox)
        {
        }

        protected override AvalonComicStoreBox CreateBox(FileInfo file)
        {
            return new AvalonComicStoreBox(file);
        }
    }
    public class AvalonStorableComicSnapshotInfo : ComicSnapshotInfo<AvalonStorableComicSourceInfo>
    {
        public AvalonStorableComicSnapshotInfo(ComicSnapshot snapshot) : base(snapshot)
        {
        }

        protected override AvalonStorableComicSourceInfo CreateSourceInfo(ComicSnapshot snapshot, ComicSource source, ComicEngine engine)
        {
            var store = AppEngine.GetRequiredService<AvalonComicStoreService>();
            var box = store.GetStoreBox(source.TargetUrl);
            return new AvalonStorableComicSourceInfo(snapshot, source, engine.GetComicSourceProviderType(source.TargetUrl), box);
        }
    }
}