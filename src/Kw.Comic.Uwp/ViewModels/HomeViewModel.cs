﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Uwp.Models;
using Kw.Comic.Uwp.Pages;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Kw.Comic.Uwp.ViewModels
{
    public class HomeViewModel : ViewModelBase
    {
        public const int PageSize = 40;

        private readonly CoreDispatcher dispatcher;
        private readonly SearchEngine searchEngine;
        private readonly ComicEngine comicEngine;


        public HomeViewModel(SearchEngine searchEngine,
            ComicEngine comicEngine)
        {
            dispatcher = Window.Current.Dispatcher;
            this.comicEngine = comicEngine;
            this.searchEngine = searchEngine;
            ComicSnapshots = new ObservableCollection<ComicSnapshotInfo>();
            CopySourceCommand = new RelayCommand(CopySource);
            OpenSourceCommand = new RelayCommand(OpenSource);
            ViewComicCommand = new RelayCommand(ViewComic);
        }
        private string keyword;
        private bool canSearch=true;
        private ComicSnapshotInfo currentSnapshot;
        private string hitokoto;
        private ComicSourceInfo currentSource;

        public ComicSourceInfo CurrentSource
        {
            get { return currentSource; }
            set => Set(ref currentSource, value);
        }

        public string Hitokoto
        {
            get { return hitokoto; }
            set => Set(ref hitokoto, value);
        }

        public ComicSnapshotInfo CurrentSnapshot
        {
            get { return currentSnapshot; }
            set
            {
                if (currentSnapshot!=null)
                {
                    currentSnapshot.SourceChanged -= CurrentSnapshot_SourceChanged;
                }
                Set(ref currentSnapshot, value);
                if (value!=null)
                {
                    currentSnapshot.SourceChanged += CurrentSnapshot_SourceChanged;
                }
                CurrentSource = null;
            }
        }

        private void CurrentSnapshot_SourceChanged(ComicSnapshotInfo arg1, ComicSourceInfo arg2)
        {
            ViewComic();
        }

        public bool CanSearch
        {
            get { return canSearch; }
            set
            {
                Set(ref canSearch, value);
            }
        }


        public string Keyword
        {
            get { return keyword; }
            set => Set(ref keyword, value);
        }


        public ICommand CopySourceCommand { get; }
        public ICommand OpenSourceCommand { get; }
        public ICommand ViewComicCommand { get; }

        public ObservableCollection<ComicSnapshotInfo> ComicSnapshots { get; }

        public void ViewComic()
        {
            if (CurrentSource!=null&& CurrentSource.CanParse && Window.Current.Content is Frame frame)
            {
                frame.Navigate(typeof(ViewPage), CurrentSource.Source.TargetUrl);
            }
        }

        public void CopySource()
        {
            if (CurrentSource != null)
            {
                var dp = new DataPackage();
                dp.SetText(CurrentSource.Source.TargetUrl);
                Clipboard.SetContent(dp);
            }
        }
        public async void OpenSource()
        {
            if (CurrentSource != null)
            {
                await Launcher.LaunchUriAsync(new Uri(CurrentSource.Source.TargetUrl));
            }
        }
        public async Task SearchAsync()
        {
            CanSearch = false;
            try
            {
                ComicSnapshots.Clear();
                CurrentSnapshot = null;
                //await Task.Delay(1000).ContinueWith(_ =>
                //{
                //    if (!CanSearch)
                //    {
                //        LoopUpdateHitokoto();
                //    }
                //});
                if (string.IsNullOrEmpty(Keyword))
                {
                    return;
                }
                var result = await searchEngine.SearchAsync(Keyword, 0, PageSize);
                if (result?.Snapshots != null)
                {
                    foreach (var item in result.Snapshots)
                    {
                        var sourceInfos = Array.Empty<ComicSourceInfo>();
                        try
                        {
                            sourceInfos = item.Sources.Select(x => new ComicSourceInfo
                            {
                                Condition = comicEngine.GetComicSourceProviderType(x.TargetUrl),
                                Source = x
                            }).ToArray();
                        }
                        catch (Exception) { }
                        ComicSnapshots.Add(new ComicSnapshotInfo
                        {
                            Snapshot = item,
                            SourceInfos = sourceInfos
                        });
                    }
                    
                }
            }
            finally
            {
                CanSearch = true;
            }
        }
    }
}
