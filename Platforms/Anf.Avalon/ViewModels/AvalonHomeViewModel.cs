﻿using Anf.Avalon.Models;
using Anf.Avalon.Services;
using Anf.Avalon.Views;
using Anf.Models;
using Anf.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using Avalonia.Media.Imaging;
using Anf.Platform.Models;

namespace Anf.Avalon.ViewModels
{
    public class AvalonHomeViewModel : HomeViewModel<AvalonStorableComicSourceInfo>
    {
        public AvalonHomeViewModel()
        {
            httpClient = AppEngine.GetRequiredService<HttpClient>();
            EngineIcons = new ObservableCollection<EngineInfo>();
            LoadEngineIcons();
        }

        public AvalonHomeViewModel(SearchEngine searchEngine,ComicEngine comicEngine)
            : base(searchEngine,comicEngine)
        {
            httpClient = AppEngine.GetRequiredService<HttpClient>();
            EngineIcons = new ObservableCollection<EngineInfo>();
            LoadEngineIcons();
        }
        private ComicSnapshotInfo<AvalonStorableComicSourceInfo> usingShapshot;
        private readonly HttpClient httpClient;

        public ObservableCollection<EngineInfo> EngineIcons { get; }
        protected override void OnBeginSearch()
        {
            DisposeSnapshot();
        }
        private async void LoadEngineIcons()
        {
            foreach (var item in ComicEngine)
            {
                try
                {
                    using (var stream = await httpClient.GetAsync(item.FaviconAddress))
                    using (var s = await stream.Content.ReadAsStreamAsync())
                    {
                        var bitmap = new Bitmap(s);
                        EngineIcons.Add(new EngineInfo { Bitmap = bitmap, Condition = item });
                    }
                }
                catch (Exception ex)
                {
                    AppEngine.GetRequiredService<ExceptionService>()
                        .Exception = ex;
                }
            }
        }
        private void DisposeSnapshot()
        {
            foreach (var item in Snapshots)
            {
                if (item is AvalonComicSnapshotInfo info && item != usingShapshot)
                {
                    info.Dispose();
                }
            }
        }
        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<AvalonStorableComicSourceInfo> info)
        {
            usingShapshot = info;
            if (info is AvalonComicSnapshotInfo sn)
            {
                var vm = new AvalonComicViewModel(info.Snapshot, sn.LogoImage);
                var navSer = AppEngine.GetRequiredService<MainNavigationService>();
                AppEngine.GetRequiredService<TitleService>().GoBackButton.IsVisible = true;

                var c =navSer.Navigate<ComicView>();
                c.DataContext = vm;
            }
        }
        
        protected override ComicSnapshotInfo<AvalonStorableComicSourceInfo> CreateSnapshotInfo(ComicSnapshot info)
        {
            return new AvalonComicSnapshotInfo(info, httpClient);
        }
        private void DisposeLogo()
        {
            foreach (var item in EngineIcons)
            {
                item.Bitmap.Dispose();
            }
            EngineIcons.Clear();
        }
        public override void Dispose()
        {
            base.Dispose();
            DisposeSnapshot();
            DisposeLogo();
        }
    }
}
