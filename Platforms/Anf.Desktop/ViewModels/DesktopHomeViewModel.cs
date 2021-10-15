using Anf.Desktop.Services;
using Anf.Desktop.Views;
using Anf.Models;
using Anf.ViewModels;
using System;
using System.Net.Http;
using Avalonia.Media.Imaging;
using Anf.Desktop.Settings;
using System.ComponentModel;
using System.Collections.Generic;
using Anf.Engine;
using Anf.Platform.Settings;
using Anf.Platform.Models.Impl;
using Anf.Networks;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Desktop.ViewModels
{
    public class DesktopHomeViewModel : HomeViewModel<WithImageStorableComicSourceInfo<Bitmap, Bitmap>, Bitmap>
    {
        private readonly List<IDisposable> subscribes = new List<IDisposable>();

        public StartupSettings StartupSettings { get; }

        public DesktopHomeViewModel()
        {
            StartupSettings = AnfSettings.Instance.Startup;
            InitDatas();
        }

        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<WithImageStorableComicSourceInfo<Bitmap, Bitmap>> info)
        {
            if (info is WithImageComicSnapshotInfo<Bitmap, Bitmap> sn)
            {
                var vm = new DesktopComicViewModel(info.Snapshot, sn.LogoImage);
                var navSer = AppEngine.GetRequiredService<MainNavigationService>();

                var c = navSer.Navigate<ComicView>();
                c.DataContext = vm;
            }
        }

        protected override ComicSnapshotInfo<WithImageStorableComicSourceInfo<Bitmap, Bitmap>> CreateSnapshotInfo(ComicSnapshot info)
        {
            var httpClient = AppEngine.GetRequiredService<INetworkAdapter>();
            return new WithImageComicSnapshotInfo<Bitmap, Bitmap>(info, httpClient);
        }


        public override void Dispose()
        {
            base.Dispose();
            foreach (var item in subscribes)
            {
                item.Dispose();
            }
            subscribes.Clear();
        }
        private async void UpdateIfNeed()
        {
            await UpdateIfNeedAsync();
        }
        protected Task UpdateIfNeedAsync()
        {
            switch (StartupSettings.StartupType)
            {
                case StartupTypes.Proposal:
                    ProposalSnapshots.Clear();
                    if (SelectedProposal is null)
                    {
                        SelectedProposal = ProposalEngine.FirstOrDefault();
                    }
                    return UpdateProposalAsync(StartupSettings.DisplayProposalCount);
                case StartupTypes.Providers:
                    return LoadEngineIcons();
                default:
                    break;
            }
            return Task.CompletedTask;
        }
        protected override void OnSelectedProposalChanged(IProposalDescription description)
        {
            _ = UpdateIfNeedAsync();
        }
        private void InitDatas()
        {
            subscribes.Add(StartupSettings.Subscribe(x => x.StartupType, UpdateIfNeed));
            subscribes.Add(StartupSettings.Subscribe(x => x.DisplayProposalCount, UpdateIfNeed));
            _ = UpdateIfNeedAsync();
        }
    }
}
