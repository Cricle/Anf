using Anf.Models;
using Anf.ViewModels;
using System;
using System.Net.Http;
using System.ComponentModel;
using System.Collections.Generic;
using Anf.Engine;
using Anf.Platform.Settings;
using Anf.Platform.Models.Impl;
using Anf.Networks;
using System.Linq;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media;
using Anf.Settings;
using Anf.Views;

namespace Anf.ViewModels
{
    internal class UnoHomeViewModel : HomeViewModel<WithImageStorableComicSourceInfo<ImageSource, ImageSource>, ImageSource>
    {
        private readonly List<IDisposable> subscribes = new List<IDisposable>();

        public StartupSettings StartupSettings { get; }

        public UnoHomeViewModel(AnfSettings settings)
        {
            StartupSettings = settings.Startup;
            InitDatas();
        }
        public UnoHomeViewModel()
        {
            StartupSettings = new StartupSettings();
            InitDatas();
        }
        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<WithImageStorableComicSourceInfo<ImageSource, ImageSource>> info)
        {
            if (info is WithImageComicSnapshotInfo<ImageSource, ImageSource> sn)
            {
                var vm = new UnoComicViewModel(info.Snapshot, sn.LogoImage);
                //var navSer = AppEngine.GetRequiredService<MainNavigationService>();

                //var c = navSer.Navigate<ComicView>();
                //c.DataContext = vm;
            }
        }

        protected override ComicSnapshotInfo<WithImageStorableComicSourceInfo<ImageSource, ImageSource>> CreateSnapshotInfo(ComicSnapshot info)
        {
            var httpClient = AppEngine.GetRequiredService<INetworkAdapter>();
            return new WithImageComicSnapshotInfo<ImageSource, ImageSource>(info, httpClient);
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
