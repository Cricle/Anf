using Anf.Platform.Models;
using System.IO;

namespace Anf.Desktop.Models
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
}
