﻿using Anf.Easy.Visiting;
using Anf.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Anf.Services;
using Anf.Platform;
using Anf.Platform.Services;
using System.Collections.Generic;
using Anf.Engine;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;

namespace Anf.ViewModels
{
    public class VisitingViewModel<TResource, TImage> : ObservableObject, IDisposable
    {
        public VisitingViewModel(IServiceProvider provider,Func<IServiceProvider, IComicVisiting<TImage>> visiting = null)
        {
            this.provider = provider;
            InitService(provider, visiting);
            InitVisiting();
        }
        public VisitingViewModel(Func<IServiceProvider, IComicVisiting<TImage>> visiting = null, bool ignoreVisting = false)
        {
            scope = AppEngine.CreateScope();
            provider = scope.ServiceProvider;
            InitService(scope.ServiceProvider, visiting,ignoreVisting);
            InitVisiting();
        }
        public VisitingViewModel(
            IComicVisiting<TImage> visiting,
            HttpClient httpClient,
            IStreamImageConverter<TImage> streamImageConverter,
            IObservableCollectionFactory observableCollectionFactory)
        {
            this.streamImageConverter = streamImageConverter ?? throw new ArgumentNullException(nameof(streamImageConverter));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.visiting = visiting ?? throw new ArgumentNullException(nameof(visiting));
            this.observableCollectionFactory = observableCollectionFactory ?? throw new ArgumentNullException(nameof(observableCollectionFactory));
            provider = visiting.Host;
            InitVisiting();
        }
        private readonly SemaphoreSlim loadSlim = new SemaphoreSlim(1);
        private IObservableCollectionFactory observableCollectionFactory;
        private CancellationTokenSource loadCancellationTokenSource;
        protected readonly IServiceScope scope;
        protected readonly IServiceProvider provider;
        protected IStreamImageConverter<TImage> streamImageConverter;
        protected HttpClient httpClient;
        protected IComicVisiting<TImage> visiting;
        private bool isLoading;
        private ChapterSlots<TImage> chapterSlots;
        private PageSlots<TImage> pageSlots;
        private IDataCursor<IComicChapterManager<TImage>> currentChaterCursor;
        private IDataCursor<IComicVisitPage<TImage>> currentPageCursor;
        private TImage logoImage;
        private ComicEntity comicEntity;
        private string name;
        private ComicChapter currentChapter;
        private ChapterWithPage currentChapterWithPage;
        private int resourceLoadCount;
        private bool resourceLoadDone;
        private bool loadingLogo;
        private bool hasLogo;
        private Stream logoStream;

        public ChapterSlots<TImage> ChapterSlots => chapterSlots;
        public CancellationTokenSource LoadCancellationTokenSource => loadCancellationTokenSource;

        private int currentPage;
        private int totalPage;
        private string chapterName;

        protected Stream LogoStream => logoStream;

        public string ChapterName
        {
            get { return chapterName; }
            private set => SetProperty(ref chapterName, value);
        }

        public int TotalPage
        {
            get { return totalPage; }
            private set => SetProperty(ref totalPage, value);
        }

        public int CurrentPage
        {
            get { return currentPage; }
            private set => SetProperty(ref currentPage, value);
        }

        public bool HasLogo
        {
            get { return hasLogo; }
            private set => SetProperty(ref hasLogo, value);
        }

        public bool LoadingLogo
        {
            get { return loadingLogo; }
            private set => SetProperty(ref loadingLogo, value);
        }

        public bool ResourceLoadDone
        {
            get { return resourceLoadDone; }
            private set => SetProperty(ref resourceLoadDone, value);
        }

        public int ResourceLoadCount
        {
            get { return resourceLoadCount; }
            private set
            {
                SetProperty(ref resourceLoadCount, value);
                ResourceLoadDone = value >= Resources.Count;
            }
        }
        public ChapterWithPage CurrentChapterWithPage
        {
            get { return currentChapterWithPage; }
            private set
            {
                SetProperty(ref currentChapterWithPage, value);
                CurrentChapter = value?.Chapter;
            }
        }

        public ComicChapter CurrentChapter
        {
            get { return currentChapter; }
            private set
            {
                SetProperty(ref currentChapter, value);
                ChapterName = value?.Title;
            }
        }

        public string Name
        {
            get { return name; }
            private set => SetProperty(ref name, value);
        }

        public PageSlots<TImage> PageSlots
        {
            get => pageSlots;
            private set => SetProperty(ref pageSlots, value);
        }

        public ComicEntity ComicEntity
        {
            get => comicEntity;
            private set
            {
                SetProperty(ref comicEntity, value);
                Name = value?.Name;
            }
        }
        public TImage LogoImage
        {
            get { return logoImage; }
            private set
            {
                SetProperty(ref logoImage, value);
                HasLogo = value != null;
            }
        }


        public IDataCursor<IComicVisitPage<TImage>> CurrentPageCursor
        {
            get { return currentPageCursor; }
            private set
            {
                SetProperty(ref currentPageCursor, value);
                OnCurrentPageCursorChanged(value);
            }
        }

        public IDataCursor<IComicChapterManager<TImage>> CurrentChaterCursor
        {
            get { return currentChaterCursor; }
            private set
            {
                SetProperty(ref currentChaterCursor, value);
            }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set
            {
                SetProperty(ref isLoading, value);
                OnLoadingChanged(value);
            }
        }
        public IComicVisiting<TImage> Visiting => visiting;

        public bool SwitchChapterGC { get; set; } = true;

        public AsyncRelayCommand FirstChapterCommand { get; protected set; }
        public AsyncRelayCommand LastChapterCommand { get; protected set; }
        public AsyncRelayCommand PrevChapterCommand { get; protected set; }
        public AsyncRelayCommand NextChapterCommand { get; protected set; }
        public AsyncRelayCommand<ComicChapter> GoChapterCommand { get; protected set; }
        public AsyncRelayCommand<int> GoChapterIndexCommand { get; protected set; }

        public AsyncRelayCommand FirstPageCommand { get; protected set; }
        public AsyncRelayCommand LastPageCommand { get; protected set; }
        public AsyncRelayCommand PrevPageCommand { get; protected set; }
        public AsyncRelayCommand NextPageCommand { get; protected set; }
        public AsyncRelayCommand<ComicPage> GoPageCommand { get; protected set; }
        public AsyncRelayCommand<int> GoPageIndexCommand { get; protected set; }

        public AsyncRelayCommand OpenComicCommand { get; protected set; }
        public AsyncRelayCommand OpenChapterCommand { get; protected set; }
        public RelayCommand CopyTitleCommand { get; protected set; }
        public RelayCommand CopyComicCommand { get; protected set; }
        public RelayCommand CopyChapterCommand { get; protected set; }
        public RelayCommand CopyComicEntityCommand { get; protected set; }

        private static IPlatformService PlatformService => AppEngine.GetRequiredService<IPlatformService>();

        public IList<ComicPageInfo<TImage>> Resources { get; protected set; }


        public event Action<IDataCursor<IComicVisitPage<TImage>>, int> PageCursorMoved;

        public Task OpenComicAsync()
        {
            return PlatformService.OpenAddressAsync(ComicEntity.ComicUrl);
        }
        public Task OpenChapterAsync()
        {
            if (CurrentChapter is null)
            {
                return TaskHelper.GetComplatedTask();
            }
            return PlatformService.OpenAddressAsync(CurrentChapter.TargetUrl);
        }


        public void CopyComic()
        {
            PlatformService.Copy(ComicEntity.ComicUrl);
        }
        public void CopyTitle()
        {
            PlatformService.Copy(ComicEntity.Name);
        }
        public void CopyComicEntity()
        {
            var str = JsonHelper.Serialize(ComicEntity);
            PlatformService.Copy(str);
        }
        public void CopyChapter()
        {
            PlatformService.Copy(CurrentChapter.TargetUrl);
        }

        protected void InitService(IServiceProvider provider, Func<IServiceProvider, IComicVisiting<TImage>> visiting = null, bool ignoreVisiting = false)
        {
            if (!ignoreVisiting)
            {
                this.visiting = visiting?.Invoke(provider) ?? provider.GetRequiredService<IComicVisiting<TImage>>();
            }
            httpClient = provider.GetRequiredService<HttpClient>();
            streamImageConverter = provider.GetRequiredService<IStreamImageConverter<TImage>>();
            observableCollectionFactory = provider.GetRequiredService<IObservableCollectionFactory>();
        }
        protected virtual void OnLoadingChanged(bool loading)
        {

        }
        protected virtual void OnCurrentPageCursorChanged(IDataCursor<IComicVisitPage<TImage>> cursor)
        {

        }
        protected virtual void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<TImage>> cursor)
        {

        }
        public async Task<IReadOnlyList<ComicPageInfo<TImage>>> LoadRangeAsync(int start, int count)
        {
            var loaded = new List<ComicPageInfo<TImage>>(Math.Min(Resources.Count, count));
            foreach (var item in Resources.Skip(start).Take(count))
            {
                if (loadCancellationTokenSource.IsCancellationRequested)
                {
                    break;
                }
                await item.LoadAsync();
                loaded.Add(item);
            }
            return loaded;
        }
        public async Task<int> LoadAllAsync()
        {
            var count = Resources.Count;
            var enu = Resources.GetEnumerator();
            while (enu.MoveNext() && (loadCancellationTokenSource==null||!loadCancellationTokenSource.IsCancellationRequested))
            {
                await enu.Current.LoadAsync();
            }
            return count;
        }
        public ComicPageInfo<TImage> GetResource(int i)
        {
            var res = Resources;
            if (i >= 0 && i < res.Count)
            {
                return res[i];
            }
            return null;
        }
        public Task LoadResourceAsync(int i)
        {
            var res = GetResource(i);
            if (res!=null)
            {
                return res.LoadAsync();
            }
            return TaskHelper.GetComplatedTask();
        }
        #region CursorMove
        public Task<bool> FirstPageAsync()
        {
            var cur = CurrentPageCursor;
            if (cur != null)
            {
                return cur.MoveFirstAsync();
            }
            return Task.FromResult(false);
        }
        public Task<bool> LastPageAsync()
        {
            var cur = CurrentPageCursor;
            if (cur != null)
            {
                return cur.MoveLastAsync();
            }
            return Task.FromResult(false);
        }
        public Task<bool> NextPageAsync()
        {
            var cur = CurrentPageCursor;
            if (cur != null)
            {
                return cur.MoveNextAsync();
            }
            return Task.FromResult(false);
        }
        public Task<bool> PrevPageAsync()
        {
            var cur = CurrentPageCursor;
            if (cur != null)
            {
                return cur.MovePrevAsync();
            }
            return Task.FromResult(false);
        }
        public Task<bool> GoPageIndexAsync(int index)
        {
            var cur = CurrentPageCursor;
            if (cur != null)
            {
                return cur.MoveAsync(index);
            }
            return Task.FromResult(false);
        }
        public Task<bool> GoPageAsync(ComicPage page)
        {
            var chCur = CurrentChaterCursor?.Current;
            if (chCur != null)
            {
                var index = Array.FindIndex(chCur.ChapterWithPage.Pages, x => x == page);
                return GoPageIndexAsync(index);
            }
            return Task.FromResult(false);
        }
        public Task<bool> GoChapterIndexAsync(int index)
        {
            var cur = CurrentChaterCursor;
            if (cur != null)
            {
                return cur.MoveAsync(index);
            }
            return Task.FromResult(false);
        }
        public Task<bool> GoChapterAsync(ComicChapter chapter)
        {
            var index = Array.FindIndex(Visiting.Entity.Chapters, x => x == chapter);
            return GoChapterIndexAsync(index);
        }
        public Task<bool> FirstChapterAsync()
        {
            var cur = CurrentChaterCursor;
            if (cur != null)
            {
                return cur.MoveFirstAsync();
            }
            return Task.FromResult(false);
        }
        public Task<bool> LastChapterAsync()
        {
            var cur = CurrentChaterCursor;
            if (cur != null)
            {
                return cur.MoveLastAsync();
            }
            return Task.FromResult(false);
        }
        public Task<bool> NextChapterAsync()
        {
            var cur = CurrentChaterCursor;
            if (cur != null)
            {
                return cur.MoveNextAsync();
            }
            return Task.FromResult(false);
        }
        public Task<bool> PrevChapterAsync()
        {
            var cur = CurrentChaterCursor;
            if (cur != null)
            {
                return cur.MovePrevAsync();
            }
            return Task.FromResult(false);
        }
        #endregion
        private void InitVisiting()
        {
            FirstChapterCommand = new AsyncRelayCommand(FirstChapterAsync);
            LastChapterCommand = new AsyncRelayCommand(LastChapterAsync);
            PrevChapterCommand = new AsyncRelayCommand(PrevChapterAsync);
            NextChapterCommand = new AsyncRelayCommand(NextChapterAsync);
            GoChapterCommand = new AsyncRelayCommand<ComicChapter>(GoChapterAsync);
            GoChapterIndexCommand = new AsyncRelayCommand<int>(GoChapterIndexAsync);

            FirstPageCommand = new AsyncRelayCommand(FirstPageAsync);
            LastPageCommand = new AsyncRelayCommand(LastPageAsync);
            PrevPageCommand = new AsyncRelayCommand(PrevPageAsync);
            NextPageCommand = new AsyncRelayCommand(NextPageAsync);
            GoPageCommand = new AsyncRelayCommand<ComicPage>(GoPageAsync);
            GoPageIndexCommand = new AsyncRelayCommand<int>(GoPageIndexAsync);

            OpenComicCommand = new AsyncRelayCommand(OpenComicAsync);
            OpenChapterCommand = new AsyncRelayCommand(OpenChapterAsync);
            CopyTitleCommand = new RelayCommand(CopyTitle);
            CopyComicCommand = new RelayCommand(CopyComic);
            CopyComicEntityCommand = new RelayCommand(CopyComicEntity);
            CopyChapterCommand = new RelayCommand(CopyChapter);

            Resources = observableCollectionFactory.Create<ComicPageInfo<TImage>>();
            if (Visiting.IsLoad())
            {
                _ = Init();
            }
            Visiting.Loading += OnLoading;
            Visiting.Loaded += OnLoaded;
            OnInitedVisiting();
        }
        protected virtual void OnInitedVisiting()
        {

        }
        private async void OnLoaded(ComicVisiting<TImage> arg1, ComicEntity arg2)
        {
            await Init();
            IsLoading = false;
        }

        private void OnLoading(ComicVisiting<TImage> arg1, string arg2)
        {
            IsLoading = true;
        }

        protected async Task Init()
        {
            try
            {
                chapterSlots = Visiting.CreateChapterSlots();
                CurrentChaterCursor = chapterSlots.ToDataCursor();
                CurrentChaterCursor.Moving += OnCurrentChaterCursorMoving;
                CurrentChaterCursor.Moved += OnCurrentChaterCursorMoved;
                CurrentChaterCursor.MoveComplated += OnCurrentChaterCursorMoveComplated;
                ComicEntity = Visiting.Entity;
                if (!string.IsNullOrEmpty(ComicEntity.ImageUrl))
                {
                    await LoadLogoAsync(ComicEntity.ImageUrl);
                }
            }
            finally
            {
                OnInitDone();

            }
        }

        private void OnCurrentChaterCursorMoveComplated(IDataCursor<IComicChapterManager<TImage>> arg1, int arg2)
        {
            IsLoading = false;
        }

        private void OnCurrentChaterCursorMoving(IDataCursor<IComicChapterManager<TImage>> arg1, int arg2)
        {
            IsLoading = true;
        }

        protected virtual void OnInitDone()
        {

        }
        protected virtual void OnCurrentChaterCursorMovedError(IDataCursor<IComicChapterManager<TImage>> arg1, int arg2,Exception ex)
        {

        }
        private async void OnCurrentChaterCursorMoved(IDataCursor<IComicChapterManager<TImage>> arg1, int arg2)
        {
            try
            {
                loadCancellationTokenSource?.Cancel();
                loadCancellationTokenSource?.Dispose();
                if (!DoNotDisposeVisiting)
                {
                    foreach (var item in Resources)
                    {
                        if (item.Resource is IDisposable disposable)
                        {
                            disposable.Dispose();
                        }
                        item.LoadDone -= OnItemLoadDone;
                    }
                }
                ResourceLoadCount = 0;
                Resources.Clear();

                var ps = PageSlots;
                if (ps != null)
                {
                    if (!DoNotDisposeVisiting)
                    {
                        ps.Dispose();
                    }
                    PageSlots = null;
                }
                var cpc = CurrentPageCursor;
                if (cpc != null)
                {
                    CurrentPageCursor.Moved -= OnCurrentPageCursorMoved;
                    if (!DoNotDisposeVisiting)
                    {
                        cpc.Dispose();
                    }
                }
                CurrentPage = 0;
                ps = ChapterSlots[arg2].CreatePageSlots();
                PageSlots = ps;
                var datas = Enumerable.Range(0, PageSlots.Size)
                    .Select(x => CreatePageInfo(ps, x));
                observableCollectionFactory.AddRange(Resources, datas);
                foreach (var item in Resources)
                {
                    item.LoadDone += OnItemLoadDone;
                }
                var pageCursor = PageSlots.ToDataCursor();
                await pageCursor.MoveNextAsync();
                CurrentPageCursor = pageCursor;
                TotalPage = pageCursor.Count;
                CurrentChapterWithPage = PageSlots.ChapterManager.ChapterWithPage;
                CurrentPageCursor.Moved += OnCurrentPageCursorMoved;
                loadCancellationTokenSource = new CancellationTokenSource();
                OnCurrentChaterCursorChanged(arg1);
                if (SwitchChapterGC)
                {
                    GC.Collect(0, GCCollectionMode.Optimized);
                }
            }
            catch (Exception ex)
            {
                OnCurrentChaterCursorMovedError(arg1, arg2, ex);
            }
        }
        private void OnItemLoadDone(ComicPageInfo<TImage> obj)
        {
            loadSlim.Wait();
            try
            {
                ResourceLoadCount++;
            }
            finally
            {
                loadSlim.Release();
            }
        }

        private void OnCurrentPageCursorMoved(IDataCursor<IComicVisitPage<TImage>> arg1, int arg2)
        {
            CurrentPage = arg2;
            PageCursorMoved?.Invoke(arg1, arg2);
        }

        protected virtual ComicPageInfo<TImage> CreatePageInfo(PageSlots<TImage> slots,int index)
        {
            return new ComicPageInfo<TImage>(slots, index);
        }
        protected virtual Task<bool> LoadComicAsync(string address)
        {
            return Visiting.LoadAsync(address);
        }
        public async Task LoadAsync(string address)
        {
            IsLoading = true;
            try
            {
                if (!DoNotDisposeVisiting)
                {
                    chapterSlots?.Dispose();
                }
                chapterSlots = null;
                CurrentChaterCursor = null;
                var ok = await LoadComicAsync(address);
                if (ok)
                {
                    await Init();
                    await OnLoadedAsync(address);
                }
            }
            finally
            {
                IsLoading = false;
            }
        }
        protected virtual Task OnLoadedAsync(string address)
        {
            return TaskHelper.GetComplatedTask();
        }

        public Task<bool> SelectChapterAsync(int index)
        {
            var cur = CurrentChaterCursor;
            if (cur != null)
            {
                return cur.MoveAsync(index);
            }
            return Task.FromResult(false);
        }
        protected virtual Task OnSelectedChapterAsync(int index)
        {
            return TaskHelper.GetComplatedTask();
        }
        public bool DoNotDisposeVisiting { get; set; }
        public virtual void Dispose()
        {
            Visiting.Loading -= OnLoading;
            Visiting.Loaded -= OnLoaded;
            try
            {
                if (!DoNotDisposeVisiting)
                {
                    Visiting.Dispose();
                    chapterSlots?.Dispose();
                    pageSlots?.Dispose();
                }
                logoStream?.Dispose();
                scope?.Dispose();
                loadSlim.Dispose();
                loadCancellationTokenSource?.Dispose();
            }
            catch (Exception) { }
        }

        private async Task LoadLogoAsync(string address)
        {
            LoadingLogo = true;
            try
            {
                var r = await OnLoadingLogoAsync(address);
                if (r)
                {
                    await OnLoadedLogoAsync(address, false);
                    return;
                }
                if (!DoNotDisposeVisiting && LogoImage is IDisposable disposable)
                {
                    disposable.Dispose();
                }
                var setting = StoreFetchSettings.DefaultCache.Clone();
                setting.DisposeStream = false;
                setting.DisposeStream = false;
                LogoImage = await StoreFetchHelper.GetOrFromCacheAsync<TResource, TImage>(address,
                    async () => logoStream = await httpClient.GetStreamAsync(address),
                    s => streamImageConverter.ToImageAsync(s),
                    setting);
                await OnLoadedLogoAsync(address, true);
            }
            finally
            {
                LoadingLogo = false;
            }
        }
        protected virtual Task<bool> OnLoadingLogoAsync(string address)
        {
            return Task.FromResult(false);
        }
        protected virtual Task OnLoadedLogoAsync(string address, bool isDefault)
        {
            return TaskHelper.GetComplatedTask();
        }
    }
}
