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
    public class AvalonHomeViewModel : HomeViewModel<AvalonStorableComicSourceInfo,Bitmap>
    {
        public AvalonHomeViewModel()
        {
            httpClient = AppEngine.GetRequiredService<HttpClient>();
            _=LoadEngineIcons((x,ex)=> 
            {
                AppEngine.GetRequiredService<ExceptionService>()
                    .Exception = ex;
            });
            _ = UpdateProposalAsync();
        }

        private ComicSnapshotInfo<AvalonStorableComicSourceInfo> usingShapshot;
        private readonly HttpClient httpClient;

        protected override void OnBeginSearch()
        {
            DisposeSnapshot();
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

                var c = navSer.Navigate<ComicView>();
                c.DataContext = vm;
            }
        }

        protected override ComicSnapshotInfo<AvalonStorableComicSourceInfo> CreateSnapshotInfo(ComicSnapshot info)
        {
            return new AvalonComicSnapshotInfo(info, httpClient);
        }
        
        public override void Dispose()
        {
            base.Dispose();
            DisposeSnapshot();
        }
    }
}
