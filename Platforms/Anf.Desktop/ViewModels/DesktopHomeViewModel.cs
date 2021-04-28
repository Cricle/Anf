using Anf.Desktop.Models;
using Anf.Desktop.Services;
using Anf.Desktop.Views;
using Anf.Models;
using Anf.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using Avalonia.Media.Imaging;
using Anf.Platform.Models;
using Avalonia.Collections;
using System.Threading.Tasks;
using Anf.Platform.Services;
using Anf.Desktop.Settings;
using System.ComponentModel;
using System.Collections.Generic;

namespace Anf.Desktop.ViewModels
{
    public class DesktopHomeViewModel : HomeViewModel<AvalonStorableComicSourceInfo, Bitmap>
    {
        public DesktopHomeViewModel()
        {
            StartupSettings = AppEngine.GetRequiredService<AnfSettings>().Startup;
            InitDatas();
        }

        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<AvalonStorableComicSourceInfo> info)
        {
            if (info is AvalonComicSnapshotInfo sn)
            {
                var vm = new DesktopComicViewModel(info.Snapshot, sn.LogoImage);
                var navSer = AppEngine.GetRequiredService<MainNavigationService>();

                var c = navSer.Navigate<ComicView>();
                c.DataContext = vm;
            }
        }

        protected override ComicSnapshotInfo<AvalonStorableComicSourceInfo> CreateSnapshotInfo(ComicSnapshot info)
        {
            var httpClient = AppEngine.GetRequiredService<HttpClient>();
            return new AvalonComicSnapshotInfo(info, httpClient);
        }

        public StartupSettings StartupSettings { get; }

        private readonly List<IDisposable> subscribes=new List<IDisposable>();
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
        private void InitDatas()
        {
            subscribes.Add(StartupSettings.Subscribe(x => x.StartupType, UpdateIfNeedAsync));
            subscribes.Add(StartupSettings.Subscribe(x => x.DisplayProposalCount, UpdateIfNeedAsync));
            UpdateIfNeedAsync();
        }
    }
}
