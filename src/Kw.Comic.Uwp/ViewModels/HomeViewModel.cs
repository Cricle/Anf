using GalaSoft.MvvmLight;
using Kw.Comic.Engine;
using Kw.Comic.Uwp.Managers;
using Kw.Comic.Uwp.Models;
using Kw.Comic.Uwp.Pages;
using Kw.Core.Annotations;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Kw.Comic.Uwp.ViewModels
{
    [EnableService(ServiceLifetime = ServiceLifetime.Singleton)]
    public class HomeViewModel : ViewModelBase
    {
        public const int PageSize = 40;

        private readonly CoreDispatcher dispatcher;
        private readonly SearchEngine searchEngine;
        private readonly ComicEngine comicEngine;
        private readonly BriefRemarkManager briefRemarkManager;


        public HomeViewModel(SearchEngine searchEngine,
            ComicEngine comicEngine,
            BriefRemarkManager briefRemarkManager)
        {
            dispatcher = Window.Current.Dispatcher;
            this.comicEngine = comicEngine;
            this.briefRemarkManager = briefRemarkManager;
            this.searchEngine = searchEngine;
            ComicSnapshots = new ObservableCollection<ComicSnapshotInfo>();
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
            //TODO:
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
        public ObservableCollection<ComicSnapshotInfo> ComicSnapshots { get; }

        public async void LoopUpdateHitokoto()
        {
            while (!CanSearch)
            {
                try
                {
                    var entity = await briefRemarkManager.GetBriefRemarkAsync();
                    await dispatcher.RunAsync(CoreDispatcherPriority.Normal, () => Hitokoto = entity.Hitokoto);
                    await Task.Delay(3000);
                }
                catch (Exception ex)
                {
                }
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
