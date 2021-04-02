using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kw.Comic.ViewModels
{
    public class HomeViewModel : ViewModelBase,IDisposable
    {
        public const int PageSize = 50;


        public HomeViewModel()
            :this(AppEngine.GetRequiredService<SearchEngine>())
        {

        }

        public HomeViewModel(SearchEngine searchEngine)
        {
            SearchEngine = searchEngine;
            Snapshots = new  SilentObservableCollection<ComicSnapshotInfo>();
            MoveNextCommand = new RelayCommand(() => _ = MoveNextAsync());
            SearchCommand = new RelayCommand(() => _ = SearchAsync());
        }
        private IComicCursor comicCursor;
        private string keyword;
        private bool emptySet=true;
        private bool searching;
        private int additionCount;
        private SearchComicResult searchResult;
        private ComicSnapshotInfo currentComicSnapshot;

        public ComicSnapshotInfo CurrentComicSnapshot
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
            set => Set(ref keyword, value);
        }

        /// <summary>
        /// 搜索引擎
        /// </summary>
        public SearchEngine SearchEngine { get; }
        /// <summary>
        /// 搜索命令
        /// </summary>
        public ICommand SearchCommand { get; }
        /// <summary>
        /// 下一页命令
        /// </summary>
        public ICommand MoveNextCommand { get; }
        /// <summary>
        /// 漫画快照
        /// </summary>
        public SilentObservableCollection<ComicSnapshotInfo> Snapshots { get; }
        /// <summary>
        /// 执行搜索
        /// </summary>
        /// <returns></returns>
        public async Task SearchAsync()
        {
            Searching = true;
            try
            {
                OnBeginSearch();
                comicCursor?.Dispose();
                Snapshots.Clear();
                comicCursor = await SearchEngine.GetSearchCursorAsync(Keyword, 0, PageSize);
                await MoveNextAsync();
                InsertDatas();
                OnEndSearch();
            }
            finally
            {
                Searching = false;
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
            var sn = comicCursor?.Current?.Snapshots;
            if (sn != null)
            {
                Snapshots.AddRange(sn.Select(x => CreateSnapshotInfo(x)));
                SearchResult = comicCursor.Current;
                AdditionCount = sn.Length;
            }
            else
            {
                SearchResult = null;
                AdditionCount = 0;
            }
            EmptySet = Snapshots.Count == 0;
        }
        protected virtual ComicSnapshotInfo CreateSnapshotInfo(ComicSnapshot info)
        {
            return new ComicSnapshotInfo(info);
        }
        protected virtual void OnSearchingChanged(bool res)
        {

        }
        protected virtual void OnCurrentComicSnapshotChanged(ComicSnapshotInfo info)
        {

        }
        /// <summary>
        /// 下一页
        /// </summary>
        /// <returns></returns>
        public async Task MoveNextAsync()
        {
            if (comicCursor != null)
            {
                await comicCursor.MoveNextAsync();
                InsertDatas();
            }
        }

        public virtual void Dispose()
        {
            comicCursor?.Dispose();
        }
    }

}
