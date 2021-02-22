using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using Kw.Comic.Engine;
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

namespace Kw.Comic.Wpf.ViewModels
{
    public class HomeViewModel:ViewModelBase
    {
        public HomeViewModel()
        {
            searchEngine = WpfAppEngine.Instance.GetRequiredService<SearchEngine>();
            comicEngine = WpfAppEngine.Instance.GetRequiredService<ComicEngine>();

            SearchCommand = new RelayCommand(Search);

            Snapshots = new ObservableCollection<ComicSnapshotInfo>();
        }
        private readonly SearchEngine searchEngine;
        private readonly ComicEngine comicEngine;

        private string keyword;
        private Visibility loadingVisibility = Visibility.Collapsed;
        private ComicSnapshotInfo currentComicSnapshot;

        public ComicSnapshotInfo CurrentComicSnapshot
        {
            get { return currentComicSnapshot; }
            set => Set(ref currentComicSnapshot, value);
        }

        public Visibility LoadingVisibility
        {
            get { return loadingVisibility; }
            set => Set(ref loadingVisibility, value);
        }


        public string Keyword
        {
            get { return keyword; }
            set => Set(ref keyword, value);
        }

        public ObservableCollection<ComicSnapshotInfo> Snapshots { get; }

        public ICommand SearchCommand { get; }

        public async void Search()
        {
            LoadingVisibility = Visibility.Visible;
            try
            {
                Snapshots.Clear();
                var res = await searchEngine.SearchAsync(Keyword, 0, 40);
                foreach (var item in res.Snapshots)
                {
                    var info = new ComicSnapshotInfo
                    {
                        Snapshot = item,
                        SourceInfos = item.Sources.Select(x =>
                        {
                            var sinfo = new ComicSourceInfo();
                            sinfo.Condition = comicEngine.GetComicSourceProviderType(x.TargetUrl);
                            sinfo.Source = x;
                            return sinfo;
                        }).ToArray()
                    };
                    Snapshots.Add(info);
                }
            }
            finally
            {
                LoadingVisibility = Visibility.Collapsed;
            }
        }
    }
}
