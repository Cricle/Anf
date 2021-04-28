using Anf.Desktop.Services;
using Anf.Models;
using Anf.Platform.Models;
using Anf.Platform.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Desktop.Models
{
    public class AvalonStorableComicSnapshotInfo : ComicSnapshotInfo<AvalonStorableComicSourceInfo>
    {
        public AvalonStorableComicSnapshotInfo(ComicSnapshot snapshot) : base(snapshot)
        {
        }

        protected override AvalonStorableComicSourceInfo CreateSourceInfo(ComicSnapshot snapshot, ComicSource source, ComicEngine engine)
        {
            var store = AppEngine.GetRequiredService<DesktopComicStoreService>();
            var box = store.GetStoreBox(source.TargetUrl);
            return new AvalonStorableComicSourceInfo(snapshot, source, engine.GetComicSourceProviderType(source.TargetUrl), box);
        }
    }
}
