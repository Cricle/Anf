using Anf.Models;
using Anf.Phone.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace Anf.Phone.Models
{
    public class PhoneStorableComicSnapshotInfo : ComicSnapshotInfo<PhoneStorableComicSourceInfo>
    {
        public PhoneStorableComicSnapshotInfo(ComicSnapshot snapshot) : base(snapshot)
        {
        }

        protected override PhoneStorableComicSourceInfo CreateSourceInfo(ComicSnapshot snapshot, ComicSource source, ComicEngine engine)
        {
            var store = AppEngine.GetRequiredService<PhoneComicStoreService>();
            var box = store.GetStoreBox(source.TargetUrl);
            return new PhoneStorableComicSourceInfo(snapshot, source, engine.GetComicSourceProviderType(source.TargetUrl), box);
        }
    }
}
