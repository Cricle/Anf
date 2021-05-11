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

namespace Anf.Desktop.ViewModels
{
    public class DesktopHomeViewModel : HomeViewModel<WithImageStorableComicSourceInfo<Bitmap>, Bitmap>
    {
        private readonly List<IDisposable> subscribes = new List<IDisposable>();

        public StartupSettings StartupSettings { get; }

        public DesktopHomeViewModel()
        {
            StartupSettings = AppEngine.GetRequiredService<AnfSettings>().Startup;
            InitDatas();
        }

        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<WithImageStorableComicSourceInfo<Bitmap>> info)
        {
            if (info is WithImageComicSnapshotInfo<Bitmap> sn)
            {
                var vm = new DesktopComicViewModel(info.Snapshot, sn.LogoImage);
                var navSer = AppEngine.GetRequiredService<MainNavigationService>();

                var c = navSer.Navigate<ComicView>();
                c.DataContext = vm;
            }
        }

        protected override ComicSnapshotInfo<WithImageStorableComicSourceInfo<Bitmap>> CreateSnapshotInfo(ComicSnapshot info)
        {
            var httpClient = AppEngine.GetRequiredService<INetworkAdapter>();
            return new WithImageComicSnapshotInfo<Bitmap>(info, httpClient);
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
        private async void UpdateIfNeedAsync()
        {
            switch (StartupSettings.StartupType)
            {
                case StartupTypes.Proposal:
                    ProposalSnapshots.Clear();
                    await UpdateProposalAsync(StartupSettings.DisplayProposalCount);
                    break;
                case StartupTypes.Providers:
                    await LoadEngineIcons();
                    break;
                default:
                    break;
            }
        }
        protected override void OnSelectedProposalChanged(IProposalDescription description)
        {
            UpdateIfNeedAsync();
        }
        private void InitDatas()
        {
            subscribes.Add(StartupSettings.Subscribe(x => x.StartupType, UpdateIfNeedAsync));
            subscribes.Add(StartupSettings.Subscribe(x => x.DisplayProposalCount, UpdateIfNeedAsync));
            UpdateIfNeedAsync();
        }
    }
}
