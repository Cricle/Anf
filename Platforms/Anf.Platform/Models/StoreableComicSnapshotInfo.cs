using Anf.Platform.Models;
using Anf.Platform.Services;

namespace Anf.Models
{
    public class StorableComicSnapshotInfo : ComicSnapshotInfo<StorableComicSourceInfo>
    {
        public StorableComicSnapshotInfo(ComicSnapshot snapshot) : base(snapshot)
        {
        }

        protected override StorableComicSourceInfo CreateSourceInfo(ComicSnapshot snapshot, ComicSource source, ComicEngine engine)
        {
            var store = AppEngine.GetRequiredService<ComicStoreService>();
            var box=store.GetStoreBox(source.TargetUrl);
            return new StorableComicSourceInfo(snapshot, source, engine.GetComicSourceProviderType(source.TargetUrl), box);
        }
    }
}
