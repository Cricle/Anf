using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using System;

namespace Kw.Comic.Wpf.Models
{
    public class ComicSnapshotInfo : ObservableObject
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

        public bool HasSourceUri => CurrentSource != null && CurrentSource.CanParse && CurrentSource.Source?.TargetUrl != null;

        public event Action<ComicSnapshotInfo, ComicSourceInfo> SourceChanged;

    }
}
