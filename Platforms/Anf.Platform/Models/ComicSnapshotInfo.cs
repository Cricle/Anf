using GalaSoft.MvvmLight;
using Anf;
using System;
using System.Collections.Generic;
using System.Linq;
using Anf.Platform.Models;
using Anf.Platform.Services;
using Anf.Services;
using GalaSoft.MvvmLight.Command;
using Anf.Engine;

namespace Anf.Models
{
    public class ComicSnapshotInfo : ComicSnapshotInfo<ComicSourceInfo>
    {
        public ComicSnapshotInfo(ComicSnapshot snapshot) : base(snapshot)
        {
        }

        protected override ComicSourceInfo CreateSourceInfo(ComicSnapshot snapshot, ComicSource source,ComicEngine engine)
        {
            return new ComicSourceInfo(snapshot, source, engine.GetComicSourceProviderType(source.TargetUrl));
        }
    }
    public abstract class ComicSnapshotInfo<TSourceInfo>: ObservableObject
        where TSourceInfo: ComicSourceInfo
    {
        private TSourceInfo currentSource;

        public ComicSnapshotInfo()
        {
            ComicEngine = AppEngine.GetRequiredService<ComicEngine>();
            CopyAuthorizeCommand = new RelayCommand(CopyAuthorize);
            CopyDescriptCommand = new RelayCommand(CopyDescript);
            CopyNameCommand = new RelayCommand(CopyName);
            CopyEntityCommand = new RelayCommand(CopyEntity);
        }

        public ComicSnapshotInfo(ComicSnapshot snapshot)
            :this()
        {
            Snapshot = snapshot;
        }

        protected abstract TSourceInfo CreateSourceInfo(ComicSnapshot snapshot, ComicSource source, ComicEngine comicEngine);

        public TSourceInfo CurrentSource
        {
            get { return currentSource; }
            set
            {
                Set(ref currentSource, value);
                SourceChanged?.Invoke(this, value);
            }
        }
        private ComicSnapshot snapshot;
        private IReadOnlyList<TSourceInfo> sourceInfos;

        public ComicSnapshot Snapshot
        {
            get => snapshot;
            set
            {
                Set(ref snapshot, value);
                if (value is null)
                {
#if NET452|| NETSTANDARD1_4
                    SourceInfos = new TSourceInfo[0];
#else
                    SourceInfos = Array.Empty<TSourceInfo>();
#endif
                }
                else
                {
                    SourceInfos = snapshot.Sources
                        .Select(x => CreateSourceInfo(Snapshot, x, ComicEngine))
                        .ToArray();
                }
            }
        }

        public IReadOnlyList<TSourceInfo> SourceInfos
        {
            get => sourceInfos;
            private set => Set(ref sourceInfos, value);
        }

        public ComicEngine ComicEngine { get; }

        public bool HasSourceUri => CurrentSource != null && CurrentSource.CanParse && !(CurrentSource.Source?.TargetUrl is null);

        public event Action<ComicSnapshotInfo<TSourceInfo>, TSourceInfo> SourceChanged;

        private IPlatformService PlatformService => AppEngine.GetRequiredService<IPlatformService>();

        public RelayCommand CopyDescriptCommand { get; }
        public RelayCommand CopyNameCommand { get; }
        public RelayCommand CopyAuthorizeCommand { get; }
        public RelayCommand CopyEntityCommand { get; }

        public void CopyDescript()
        {
            PlatformService.Copy(Snapshot.Descript);
        }
        public void CopyName()
        {
            PlatformService.Copy(Snapshot.Name);
        }
        public void CopyAuthorize()
        {
            PlatformService.Copy(Snapshot.Author);
        }
        public void CopyEntity()
        {
            var str = JsonHelper.Serialize(Snapshot);
            PlatformService.Copy(str);
        }
    }
}
