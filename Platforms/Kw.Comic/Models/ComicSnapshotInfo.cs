using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kw.Comic.Models
{
    public class ComicSnapshotInfo : ObservableObject
    {
        private ComicSourceInfo currentSource;

        public ComicSnapshotInfo(ComicSnapshot snapshot)
        {
            Snapshot = snapshot;
            var prov = AppEngine.GetRequiredService<ComicEngine>();
            SourceInfos = snapshot.Sources
                .Select(x=> 
                {
                    var condition = prov.GetComicSourceProviderType(x.TargetUrl);
                    return new ComicSourceInfo(snapshot, x, condition);
                }).ToArray();
        }

        public ComicSourceInfo CurrentSource
        {
            get { return currentSource; }
            set
            {
                Set(ref currentSource, value);
                SourceChanged?.Invoke(this, value);
            }
        }

        public ComicSnapshot Snapshot { get; }

        public IReadOnlyList<ComicSourceInfo> SourceInfos { get; }

        public bool HasSourceUri => !(CurrentSource is null) && CurrentSource.CanParse && !(CurrentSource.Source?.TargetUrl is null);

        public event Action<ComicSnapshotInfo, ComicSourceInfo> SourceChanged;

    }
}
