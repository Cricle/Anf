using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Anf;
using Anf.Models;
using Anf.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Extensions.DependencyInjection;
using Anf.Platform.Services;

namespace Anf.ViewModels
{
    public abstract class HomeViewModel<TSourceInfo> : ViewModelBase,IDisposable
        where TSourceInfo:ComicSourceInfo
    {
        public const int PageSize = 50;


        public HomeViewModel()
        {
            ComicEngine = AppEngine.GetRequiredService<ComicEngine>();
            SearchEngine = AppEngine.GetRequiredService<SearchEngine>();
            SearchCommand = new RelayCommand(() => _ = SearchAsync());
            GoSourceCommand = new RelayCommand(GoSource);
            scope = AppEngine.CreateScope();
            var type = SearchEngine.FirstOrDefault();
            if (type != null)
            {
                CurrentSearchProvider = (ISearchProvider)scope.ServiceProvider.GetRequiredService(type);
            }
            observableCollectionFactory = AppEngine.GetRequiredService<IObservableCollectionFactory>();
            Snapshots = observableCollectionFactory.Create<ComicSnapshotInfo<TSourceInfo>>();
        }
        protected readonly IServiceScope scope;
        private string keyword;
        private bool emptySet=true;
        private bool searching;
        private int additionCount;
        private SearchComicResult searchResult;
        private ComicSnapshotInfo<TSourceInfo> currentComicSnapshot;
        private ISearchProvider currentSearchProvider;
        private int skip;
        private int take=PageSize;
        private IComicSourceCondition avaliableCondition;
        private bool hasAvaliableCondition;

        public bool HasAvaliableCondition
        {
            get { return hasAvaliableCondition; }
            private set => Set(ref hasAvaliableCondition, value);
        }

        public IComicSourceCondition AvaliableCondition
        {
            get { return avaliableCondition; }
            private set
            {
                Set(ref avaliableCondition, value);
                HasAvaliableCondition = value != null;
            }
        }

        public int Take
        {
            get { return take; }
            set => Set(ref take, value);
        }

        public int Skip
        {
            get { return skip; }
            set => Set(ref skip, value);
        }

        public ISearchProvider CurrentSearchProvider
        {
            get { return currentSearchProvider; }
            set => Set(ref currentSearchProvider, value);
        }

        public ComicSnapshotInfo<TSourceInfo> CurrentComicSnapshot
        {
            get { return currentComicSnapshot; }
            set
            {
                Set(ref currentComicSnapshot, value);
                OnCurrentComicSnapshotChanged(value);
            }
        }

        /// <summary>
        /// 搜索的结果
        /// </summary>
        public SearchComicResult SearchResult
        {
            get { return searchResult; }
            private set => Set(ref searchResult, value);
        }

        /// <summary>
        /// 新添加的数量
        /// </summary>
        public int AdditionCount
        {
            get { return additionCount; }
            private set => Set(ref additionCount, value);
        }

        /// <summary>
        /// 是否正在搜索
        /// </summary>
        public bool Searching
        {
            get { return searching; }
            private set
            {
                Set(ref searching, value);
                OnSearchingChanged(value);
            }
        }
        /// <summary>
        /// 当前是否空集合
        /// </summary>
        public bool EmptySet
        {
            get { return emptySet; }
            private set => Set(ref emptySet, value);
        }
        /// <summary>
        /// 搜索关键字
        /// </summary>
        public string Keyword
        {
            get { return keyword; }
            set 
            {
                Set(ref keyword, value);
                AvaliableCondition = null;
                if (UrlHelper.IsWebsite(value))
                {
                    try
                    {
                        var addr = value.GetUrl();
                        AvaliableCondition = ComicEngine.GetComicSourceProviderType(addr);
                    }
                    catch (Exception) { }
                }
            }
        }
        /// <summary>
        /// 漫画解析引擎
        /// </summary>
        public ComicEngine ComicEngine { get; }
        /// <summary>
        /// 搜索引擎
        /// </summary>
        public SearchEngine SearchEngine { get; }

        public HistoryService HistoryService { get; } = AppEngine.GetRequiredService<HistoryService>();
        /// <summary>
        /// 搜索命令
        /// </summary>
        public ICommand SearchCommand { get; }
        public ICommand GoSourceCommand { get; }
        /// <summary>
        /// 漫画快照
        /// </summary>
        public IList<ComicSnapshotInfo<TSourceInfo>> Snapshots { get; }
        private readonly IObservableCollectionFactory observableCollectionFactory;
        /// <summary>
        /// 执行搜索
        /// </summary>
        /// <returns></returns>
        public async Task SearchAsync()
        {
            if (CurrentSearchProvider is null)
            {
                return;
            }
            Searching = true;
            try
            {
                OnBeginSearch();
                Snapshots.Clear();
                var keyword = Keyword;
                SearchResult=await CurrentSearchProvider.SearchAsync(keyword, Skip,Take);
                InsertDatas();
                OnEndSearch();
                HistoryService.Lines.Add(keyword);
            }
            finally
            {
                Searching = false;
            }
        }
        public void GoSource()
        {
            if (HasAvaliableCondition)
            {
                var nav = AppEngine.GetRequiredService<IComicTurnPageService>();
                var addr = keyword.GetUrl();
                HistoryService.Lines.Add(addr);
                nav.GoSource(addr);
            }
        }
        protected virtual void OnBeginSearch()
        {

        }
        protected virtual void OnEndSearch()
        {

        }
        private void InsertDatas()
        {
            var sn = SearchResult.Snapshots;
            if (sn != null)
            {
                observableCollectionFactory.AddRange(Snapshots,sn.Select(x => CreateSnapshotInfo(x)));
                AdditionCount = sn.Length;
            }
            else
            {
                SearchResult = null;
                AdditionCount = 0;
            }
            EmptySet = Snapshots.Count == 0;
        }
        protected abstract ComicSnapshotInfo<TSourceInfo> CreateSnapshotInfo(ComicSnapshot info);
        protected virtual void OnSearchingChanged(bool res)
        {

        }
        protected virtual void OnCurrentComicSnapshotChanged(ComicSnapshotInfo<TSourceInfo> info)
        {

        }

        public virtual void Dispose()
        {
            scope.Dispose();
        }
    }

}
