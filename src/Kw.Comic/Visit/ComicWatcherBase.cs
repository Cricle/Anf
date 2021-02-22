using Kw.Comic.Engine;
using Kw.Core.Input;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public abstract class ComicWatcherBase<T> : ViewModelBase
        where T : ChapterVisitorBase
    {
        public ComicWatcherBase(ComicEntity comic,
            IHttpClientFactory httpClientFactory,
            IComicSourceCondition condition,
            IComicSourceProvider comicSourceProvider)
        {
            HttpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            Comic = comic ?? throw new ArgumentNullException(nameof(comic));
            Condition = condition ?? throw new ArgumentNullException(nameof(condition));
            ComicSourceProvider = comicSourceProvider ?? throw new ArgumentNullException(nameof(comicSourceProvider));
            ChapterCursor = new ChapterCursor(comic.Chapters.Select(x => new ComicVisitor(x, comicSourceProvider)));
        }

        private PageCursorBase<T> pageCursor;
        private bool isLoading;
        private bool cachePageCursor = true;
        private bool singleOperator;
        private bool forceNewPageCursor;

        public IComicChapterCacher<T> ChapterCacher { get; set; }

        public bool ForceNewPageCursor
        {
            get { return forceNewPageCursor; }
            set => RaisePropertyChanged(ref forceNewPageCursor, value);
        }

        public bool SingleOperator
        {
            get { return singleOperator; }
            set => RaisePropertyChanged(ref singleOperator, value);
        }

        public bool CachePageCursor
        {
            get { return cachePageCursor; }
            set => RaisePropertyChanged(ref cachePageCursor, value);
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set => RaisePropertyChanged(ref isLoading, value);
        }

        public PageCursorBase<T> PageCursor
        {
            get { return pageCursor; }
            private set
            {
                RaisePropertyChanged(ref pageCursor, value);
            }
        }

        public ComicEntity Comic { get; }

        public ChapterCursor ChapterCursor { get; }

        public IComicSourceCondition Condition { get; }

        public IComicSourceProvider ComicSourceProvider { get; }

        public IHttpClientFactory HttpClientFactory { get; }

        public SemaphoreSlim Locker { get; } = new SemaphoreSlim(1, 1);

        public event Action<ComicWatcherBase<T>, PageCursorBase<T>> PageCursorCreated;
        public event Action<ComicWatcherBase<T>, PageCursorBase<T>> PageCursorCacheCreated;
        public event Action<ComicWatcherBase<T>, PageCursorBase<T>> LoadedChapter;

        public void ResetCache()
        {
            ChapterCacher?.Reset();
        }

        public virtual bool CanDoes()
        {
            return !SingleOperator || !IsLoading;
        }
        /// <summary>
        /// 加载章节
        /// </summary>
        /// <param name="index">目标索引</param>
        /// <param name="forceNewPageCursor">强制使用新章节游标</param>
        /// <param name="cachePageCursor">缓存章节游标</param>
        /// <returns></returns>
        public async Task<PageCursorBase<T>> CoreLoadChapterAsync(int index,
            bool forceNewPageCursor,
            bool cachePageCursor)
        {
            if (index < 0 || index >= ChapterCursor.Length)
            {
                throw new ArgumentException($"Index must in [0, {ChapterCursor.Length})");
            }
            await Locker.WaitAsync();
            try
            {

                if (!forceNewPageCursor&&ChapterCacher!=null)
                {
                    var cursor = ChapterCacher.GetCache(index);
                    if (cursor != null)
                    {
                        return cursor;
                    }
                }
                var httpClient = HttpClientFactory.CreateClient(Condition.ImageHttpName);
                var pageCursor = await MakePageCursorAsync(index,httpClient);
                if (cachePageCursor&&ChapterCacher!=null)
                {
                    ChapterCacher.SetCache(index, pageCursor);
                    PageCursorCacheCreated?.Invoke(this, pageCursor);
                }
                PageCursorCreated?.Invoke(this, pageCursor);
                return pageCursor;
            }
            finally
            {
                Locker.Release();
            }
        }
        protected abstract Task<PageCursorBase<T>> MakePageCursorAsync(int i, HttpClient httpClient);
        protected abstract Task<PageCursorBase<T>> MakePageCursorAsync(HttpClient httpClient);

        private async Task LoadChapterAsync()
        {
            var old = PageCursor;
            var idx = ChapterCursor.Index < 0 ? 0 : ChapterCursor.Index;
            PageCursor = await CoreLoadChapterAsync(idx, ForceNewPageCursor, CachePageCursor);
            await OnLoadChapterAsync(old, PageCursor);
            if (PageCursor != null)
            {
                await PageCursor.SetFirstAsync();
            }
            LoadedChapter?.Invoke(this, PageCursor);
            if (!CachePageCursor)
            {
                old?.Dispose();
            }
        }
        protected virtual Task OnLoadChapterAsync(PageCursorBase<T> old, PageCursorBase<T> @new)
        {
            return Task.CompletedTask;
        }

        public async Task ToPageAsync(int index)
        {
            if (!CanDoes())
            {
                return;
            }
            if (PageCursor != null)
            {
                if (!CanDoes())
                {
                    return;
                }
                IsLoading = true;
                try
                {
                    await PageCursor.SetIndexAsync(index);
                }
                finally
                {
                    IsLoading = false;
                }

            }
        }

        public async Task FirstChapterAsync()
        {
            if (!CanDoes())
            {
                return;
            }
            IsLoading = true;
            try
            {
                await ChapterCursor.SetFirstAsync();
                await LoadChapterAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task ToChapterAsync(int index)
        {
            if (!CanDoes())
            {
                return;
            }
            IsLoading = true;
            try
            {
                if (ChapterCursor.Index != index)
                {
                    await ChapterCursor.SetIndexAsync(index);
                    await LoadChapterAsync();
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task LastChapterAsync()
        {
            if (!CanDoes())
            {
                return;
            }
            IsLoading = true;
            try
            {
                await ChapterCursor.SetLastAsync();
                await LoadChapterAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task NextChapterAsync()
        {
            if (!CanDoes())
            {
                return;
            }
            IsLoading = true;
            try
            {
                await ChapterCursor.NextAsync();
                await LoadChapterAsync();
            }
            finally
            {
                IsLoading = false;
            }

        }
        public async Task PrevChapterAsync()
        {
            if (!CanDoes())
            {
                return;
            }
            IsLoading = true;
            try
            {
                await ChapterCursor.PrevAsync();
                await LoadChapterAsync();
            }
            finally
            {
                IsLoading = false;
            }
        }
        public async Task PrevPageAsync()
        {
            if (PageCursor != null)
            {
                if (!CanDoes())
                {
                    return;
                }
                IsLoading = true;
                try
                {
                    var ok = await PageCursor.PrevAsync();
                    if (!ok)
                    {
                        await PrevChapterAsync();
                    }
                }
                finally
                {
                    IsLoading = false;
                }

            }
        }
        public async Task NextPageAsync()
        {
            if (PageCursor != null)
            {
                if (!CanDoes())
                {
                    return;
                }
                IsLoading = true;
                try
                {
                    var ok = await PageCursor.NextAsync();
                    if (!ok)
                    {
                        await NextChapterAsync();
                    }
                }
                finally
                {
                    IsLoading = false;
                }

            }
        }

    }
}
