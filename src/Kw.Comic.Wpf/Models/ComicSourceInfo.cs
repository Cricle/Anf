using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Wpf.Models
{
    public class ComicSnapshotInfo:ObservableObject
    {
        private ComicSourceInfo currentSource;

        public ComicSourceInfo CurrentSource
        {
            get { return currentSource; }
            set
            {
                Set(ref currentSource, value);
                SourceChanged?.Invoke(this, value);
            }
        }

        public ComicSnapshot Snapshot { get; set; }

        public ComicSourceInfo[] SourceInfos { get; set; }

        public event Action<ComicSnapshotInfo, ComicSourceInfo> SourceChanged;
    }
    public class ComicSourceInfo
    {
        public bool CanParse => Condition != null;

        public PackIconMaterialLightKind Icon => CanParse ? PackIconMaterialLightKind.Check : PackIconMaterialLightKind.AlertCircle;

        public IComicSourceCondition Condition { get; set; }

        public ComicSource Source { get; set; }
    }
}
