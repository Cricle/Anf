using Kw.Comic.Avalon.Models;
using Kw.Comic.Avalon.Views;
using Kw.Comic.Engine;
using Kw.Comic.Models;
using Kw.Comic.Services;
using Kw.Comic.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Kw.Comic.Avalon.ViewModels
{
    public class AvalonHomeViewModel : HomeViewModel
    {
        public AvalonHomeViewModel()
        {
            httpClient = AppEngine.GetRequiredService<HttpClient>();
        }

        public AvalonHomeViewModel(SearchEngine searchEngine) : base(searchEngine)
        {
            httpClient = AppEngine.GetRequiredService<HttpClient>();
        }
        private ComicSnapshotInfo usingShapshot;
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
        protected override void OnCurrentComicSnapshotChanged(ComicSnapshotInfo info)
        {
            usingShapshot = info;
            if (info is AvalonComicSnapshotInfo sn)
            {
                var vm = new AvalonComicViewModel(info.Snapshot, sn.LogoImage);
                var page = new ComicView { DataContext = vm };
                var navSer = AppEngine.GetRequiredService<INavigationService>();
                navSer.Navigate(page);
            }
        }
        protected override ComicSnapshotInfo CreateSnapshotInfo(ComicSnapshot info)
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
