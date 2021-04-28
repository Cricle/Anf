using Anf.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.WebClient.Models
{
    public class WebComicSnapshotInfo : ComicSnapshotInfo<ComicSourceInfo>
    {
        public WebComicSnapshotInfo()
        {
        }

        public WebComicSnapshotInfo(ComicSnapshot snapshot) : base(snapshot)
        {
        }

        protected override ComicSourceInfo CreateSourceInfo(ComicSnapshot snapshot, ComicSource source, ComicEngine comicEngine)
        {
            var condition = comicEngine.GetComicSourceProviderType(source.TargetUrl);
            return new ComicSourceInfo(snapshot, source, condition);
        }
    }
}
