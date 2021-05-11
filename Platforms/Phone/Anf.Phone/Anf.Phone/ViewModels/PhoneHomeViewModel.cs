using Anf.Engine;
using Anf.Models;
using Anf.Networks;
using Anf.Phone.Models;
using Anf.Phone.Settings;
using Anf.Platform.Models.Impl;
using Anf.Platform.Settings;
using Anf.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Text;
using Xamarin.Forms;

namespace Anf.Phone.ViewModels
{
    public class PhoneHomeViewModel : HomeViewModel<WithImageStorableComicSourceInfo<ImageSource>, ImageSource>
    {
        private readonly List<IDisposable> subscribes = new List<IDisposable>();

        public StartupSettings StartupSettings { get; }

        public PhoneHomeViewModel()
        {
            StartupSettings = AppEngine.GetRequiredService<AnfSettings>().Startup;
            InitDatas();
            Take = 5;
        }

        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<WithImageStorableComicSourceInfo<ImageSource>> info)
        {
            //if (info is PhoneComicSnapshotInfo sn)
            //{
            //    var vm = new PhoneComicViewModel(info.Snapshot, sn.LogoImage);
            //    var navSer = AppEngine.GetRequiredService<MainNavigationService>();

            //    var c = navSer.Navigate<ComicView>();
            //    c.DataContext = vm;
            //}
        }

        protected override ComicSnapshotInfo<WithImageStorableComicSourceInfo<ImageSource>> CreateSnapshotInfo(ComicSnapshot info)
        {
            var httpClient = AppEngine.GetRequiredService<INetworkAdapter>();
            return new WithImageComicSnapshotInfo<ImageSource>(info, httpClient);
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
