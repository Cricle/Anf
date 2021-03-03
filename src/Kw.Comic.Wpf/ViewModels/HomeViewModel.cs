using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using Kw.Comic.Wpf.Managers;
using Kw.Comic.Wpf.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Kw.Comic.Wpf.Views.Pages;
using GalaSoft.MvvmLight.Command;

namespace Kw.Comic.Wpf.ViewModels
{
    public class HomeViewModel:ViewModelBase
    {
        public HomeViewModel()
        {
            searchEngine = WpfAppEngine.Instance.GetRequiredService<SearchEngine>();
            comicEngine = WpfAppEngine.Instance.GetRequiredService<ComicEngine>();
            mainNavigationService = WpfAppEngine.Instance.GetRequiredService<MainNavigationService>();
            historyManager = WpfAppEngine.Instance.GetRequiredService<DownloadManager>();
            historys = historyManager.EnumerableComic();

            SearchCommand = new RelayCommand(Search);
            GoCommand = new RelayCommand(Go);
            NextCommand = new RelayCommand(Next);

            Snapshots = new ObservableCollection<ComicSnapshotInfo>();
            HistoryInfos = new ObservableCollection<PhysicalComicInfo>();

            UpdateHistory();
        }
        private readonly DownloadManager historyManager;
        private readonly MainNavigationService mainNavigationService;
        private readonly SearchEngine searchEngine;
        private readonly ComicEngine comicEngine;
        private readonly IEnumerable<PhysicalComicInfo> historys;

        private string keyword;
        private Visibility loadingVisibility = Visibility.Collapsed;
        private ComicSnapshotInfo currentComicSnapshot;
        private Visibility directGoVisibility= Visibility.Collapsed;
        private bool hasData;
        private int pageIndex;
        private int historIndex;
        private bool historyHasData;

        public bool HistoryHasData
        {
            get { return historyHasData; }
            set => Set(ref historyHasData, value);
        }

        public int HistorIndex
        {
            get { return historIndex; }
            set => Set(ref historIndex, value);
        }

        public int PageIndex
        {
            get { return pageIndex; }
            set => Set(ref pageIndex, value);
        }

        public bool HasData
        {
            get { return hasData; }
            set => Set(ref hasData, value);
        }

        public Visibility DirectGoVisibility
        {
            get { return directGoVisibility; }
            set => Set(ref directGoVisibility, value);
        }

        public ComicSnapshotInfo CurrentComicSnapshot
        {
            get { return currentComicSnapshot; }
            set
            {
                Set(ref currentComicSnapshot, value);
                if (value != null)
                {
                    GoComicDetail(value);
                }
            }
        }

        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set => Set(ref loadingVisibility, value);
        }


        public string Keyword
        {
            get { return keyword; }
            set
            {
                Set(ref keyword, value);
                DirectGoVisibility = Visibility.Collapsed;
                if (value != null && value.StartsWith("http://") || value.StartsWith("www."))
                {
                    try
                    {
                        var condition = comicEngine.GetComicSourceProviderType(value);
                        if (condition != null)
                        {
                            DirectGoVisibility = Visibility.Visible;
                        }
                    }
                    catch (Exception)
                    {

                    }
                }
            }
        }

        public ObservableCollection<PhysicalComicInfo> HistoryInfos { get; }

        public ObservableCollection<ComicSnapshotInfo> Snapshots { get; }

        public ICommand SearchCommand { get; }
        public ICommand GoCommand { get; }
        public ICommand UpdateHistoryCommand { get; }
        public ICommand ClearHistoryCommand { get; }
        public ICommand NextCommand { get; }

        public void UpdateHistory()
        {
            foreach (var item in historys.Take(20))
            {
                HistoryInfos.Add(item);
            }
            HistoryHasData = HistoryInfos.Count != 0;
        }

        public void ClearHistory()
        {

        }

        public void Go()
        {
            var navSer = WpfAppEngine.GetNavigationService();
            var vp = new ViewPage(Keyword);
            navSer.Frame.Navigate(vp);
        }
        public void GoComicDetail(ComicSnapshotInfo info)
        {
            var navSer = WpfAppEngine.GetNavigationService();
            var vp = new Views.Pages.ComicPage(info);
            navSer.Frame.Navigate(vp);
            mainNavigationService.SetTitle(info.Snapshot.Name);
        }
        private bool nexting;
        public async void Next()
        {
            if (nexting)
            {
                return;
            }
            nexting = true;
            try
            {
                PageIndex++;
                await LoadPageAsync();
            }
            finally
            {
                nexting = false;
            }
        }
        public async Task LoadPageAsync()
        {
            LoadingVisibility = Visibility.Visible;
            try
            {
                var res = await searchEngine.SearchAsync(Keyword, PageIndex*20, 20);
                foreach (var item in res.Snapshots)
                {
                    var info = new ComicSnapshotInfo
                    {
                        Snapshot = item,
                        SourceInfos = item.Sources.Select(x =>
                        {
                            var sinfo = new ComicSourceInfo();
                            sinfo.ComicName = item.Name;
                            sinfo.Condition = comicEngine.GetComicSourceProviderType(x.TargetUrl);
                            sinfo.Source = x;
                            return sinfo;
                        }).ToArray()
                    };
                    Snapshots.Add(info);
                }
                HasData = Snapshots.Count != 0;
            }
            finally
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }
        public async void Search()
        {
            PageIndex = 0;
            Snapshots.Clear();
            mainNavigationService.SetTitle("搜索" + Keyword);
            await LoadPageAsync();
        }
    }
}
