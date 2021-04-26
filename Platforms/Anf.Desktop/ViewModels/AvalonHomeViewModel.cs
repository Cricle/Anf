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

namespace Anf.Desktop.ViewModels
{
    public class AvalonHomeViewModel : HomeViewModel<AvalonStorableComicSourceInfo>
    {
        public AvalonHomeViewModel()
        {
            httpClient = AppEngine.GetRequiredService<HttpClient>();
            EngineIcons = new AvaloniaList<EngineInfo>();
            LoadEngineIcons();
            _ = this.UpdateProposalAsync();
        }

        private ComicSnapshotInfo<AvalonStorableComicSourceInfo> usingShapshot;
        private readonly HttpClient httpClient;

        public AvaloniaList<EngineInfo> EngineIcons { get; }
        protected override void OnBeginSearch()
        {
            DisposeSnapshot();
        }
        private async void LoadEngineIcons()
        {
            foreach (var item in ComicEngine)
            {
                var addr = item.FaviconAddress;
                if (addr is null)
                {
                    continue;
                }
                try
                {
                    using(var rep=await httpClient.GetAsync(addr))
                    using (var stream = await rep.Content.ReadAsStreamAsync())
                    {
                        var bitmap = new Bitmap(stream);
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
