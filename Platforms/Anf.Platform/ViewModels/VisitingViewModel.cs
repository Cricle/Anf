using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Anf;
using Anf.Easy.Visiting;
using Anf.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Anf.ViewModels
{
    public class VisitingViewModel<TResource, TImage> : ViewModelBase, IDisposable
    {
        public VisitingViewModel(IComicVisiting<TResource> visiting = null)
        {
            InitService(scope.ServiceProvider, visiting);
            InitVisiting();
        }
        public VisitingViewModel(
            IComicVisiting<TResource> visiting,
            HttpClient httpClient,
            RecyclableMemoryStreamManager recyclableMemoryStreamManager,
            IStreamImageConverter<TImage> streamImageConverter)
        {
            this.streamImageConverter = streamImageConverter ?? throw new ArgumentNullException(nameof(streamImageConverter));
            this.recyclableMemoryStreamManager = recyclableMemoryStreamManager ?? throw new ArgumentNullException(nameof(recyclableMemoryStreamManager));
            this.httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.visiting = visiting ?? throw new ArgumentNullException(nameof(visiting));
            InitVisiting();
        }
        private CancellationTokenSource loadCancellationTokenSource;
        protected readonly IServiceScope scope= AppEngine.CreateScope();
        protected IStreamImageConverter<TImage> streamImageConverter;
        protected RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        protected HttpClient httpClient;
        private IComicVisiting<TResource> visiting;
        private MemoryStream logoStream;
        private bool isLoading;
        private ChapterSlots<TResource> chapterSlots;
        private PageSlots<TResource> pageSlots;
        private IDataCursor<IComicChapterManager<TResource>> currentChaterCursor;
        private IDataCursor<IComicVisitPage<TResource>> currentPageCursor;
        private TImage logoImage;
        private ComicEntity comicEntity;
        private string name;
        private ComicChapter currentChapter;
        private ChapterWithPage currentChapterWithPage;

        public MemoryStream LogoStream => logoStream;
        public ChapterSlots<TResource> ChapterSlots => chapterSlots;
        public CancellationTokenSource LoadCancellationTokenSource=> loadCancellationTokenSource;
       
        public ChapterWithPage CurrentChapterWithPage
        {
            get { return currentChapterWithPage; }
            private set
            {
                Set(ref currentChapterWithPage, value);
                CurrentChapter = value?.Chapter;
            }
        }

        public ComicChapter CurrentChapter
        {
            get { return currentChapter; }
            private set => Set(ref currentChapter, value);
        }

        public string Name
        {
            get { return name; }
            private set => Set(ref name, value);
        }

        public PageSlots<TResource> PageSlots
        {
            get => pageSlots;
            private set => Set(ref pageSlots, value);
        }

        public ComicEntity ComicEntity
        {
            get => comicEntity;
            private set
            {
                Set(ref comicEntity, value);
                Name = value?.Name;
            }
        }
        public TImage LogoImage
        {
            get { return logoImage; }
            private set
            {
                Set(ref logoImage, value);
            }
        }


        public IDataCursor<IComicVisitPage<TResource>> CurrentPageCursor
        {
            get { return currentPageCursor; }
            private set
            {
                Set(ref currentPageCursor, value);
                OnCurrentPageCursorChanged(value);
            }
        }

        public IDataCursor<IComicChapterManager<TResource>> CurrentChaterCursor
        {
            get { return currentChaterCursor; }
            private set
            {
                Set(ref currentChaterCursor, value);
            }
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set
            {
                Set(ref isLoading, value);
                OnLoadingChanged(value);
            }
        }
        public IComicVisiting<TResource> Visiting => visiting;


        public RelayCommand FirstChapterCommand { get; protected set; }
        public RelayCommand LastChapterCommand { get; protected set; }
        public RelayCommand PrevChapterCommand { get; protected set; }
        public RelayCommand NextChapterCommand { get; protected set; }
        public RelayCommand<ComicChapter> GoChapterCommand { get; protected set; }
        public RelayCommand<int> GoChapterIndexCommand { get; protected set; }

        public RelayCommand FirstPageCommand { get; protected set; }
        public RelayCommand LastPageCommand { get; protected set; }
        public RelayCommand PrevPageCommand { get; protected set; }
        public RelayCommand NextPageCommand { get; protected set; }
        public RelayCommand<ComicPage> GoPageCommand { get; protected set; }
        public RelayCommand<int> GoPageIndexCommand { get; protected set; }
        public SilentObservableCollection<ComicPageInfo<TResource>> Resources { get; protected set; }

        protected void InitService(IServiceProvider provider, IComicVisiting<TResource> visiting = null)
        {
            this.visiting = visiting ?? provider.GetRequiredService<IComicVisiting<TResource>>();
            httpClient = provider.GetRequiredService<HttpClient>();
            recyclableMemoryStreamManager = provider.GetRequiredService<RecyclableMemoryStreamManager>();
            streamImageConverter = provider.GetRequiredService<IStreamImageConverter<TImage>>();
        }
        protected virtual void OnLoadingChanged(bool loading)
        {

        }
        protected virtual void OnCurrentPageCursorChanged(IDataCursor<IComicVisitPage<TResource>> cursor)
        {

        }
        protected virtual void OnCurrentChaterCursorChanged(IDataCursor<IComicChapterManager<TResource>> cursor)
        {

        }
        public async Task<int> LoadAllAsync()
        {
            var count = Resources.Count;
            var enu = Resources.GetEnumerator();
            while (enu.MoveNext()&& !loadCancellationTokenSource.IsCancellationRequested)
            {
                await enu.Current.LoadAsync();
            }
            return count;
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
            FirstChapterCommand = new RelayCommand(() => _ = FirstChapterAsync());
            LastChapterCommand = new RelayCommand(() => _ = LastChapterAsync());
            PrevChapterCommand = new RelayCommand(() => _ = PrevChapterAsync());
            NextChapterCommand = new RelayCommand(() => _ = NextChapterAsync());
            GoChapterCommand = new RelayCommand<ComicChapter>(x => _ = GoChapterAsync(x));
            GoChapterIndexCommand = new RelayCommand<int>(x => _ = GoChapterIndexAsync(x));

            FirstPageCommand = new RelayCommand(() => _ = FirstPageAsync());
            LastPageCommand = new RelayCommand(() => _ = LastPageAsync());
            PrevPageCommand = new RelayCommand(() => _ = PrevPageAsync());
            NextPageCommand = new RelayCommand(() => _ = NextPageAsync());
            GoPageCommand = new RelayCommand<ComicPage>(x => _ = GoPageAsync(x));
            GoPageIndexCommand = new RelayCommand<int>(x => _ = GoPageIndexAsync(x));

            Resources = new SilentObservableCollection<ComicPageInfo<TResource>>();
            if (Visiting.IsLoad())
            {
                _ = Init();
            }
            Visiting.Loading += OnLoading;
            Visiting.Loaded += OnLoaded;
        }

        private async void OnLoaded(ComicVisiting<TResource> arg1, ComicEntity arg2)
        {
            await Init();
            IsLoading = false;
        }

        private void OnLoading(ComicVisiting<TResource> arg1, string arg2)
        {
            IsLoading = true;
        }

        protected async Task Init()
        {
            chapterSlots = Visiting.CreateChapterSlots();
            CurrentChaterCursor = chapterSlots.ToDataCursor();
            CurrentChaterCursor.Moved += OnCurrentChaterCursorMoved;
            ComicEntity = Visiting.Entity;
            if (!string.IsNullOrEmpty(ComicEntity.ImageUrl))
            {
                await LoadLogoAsync(ComicEntity.ImageUrl);
            }
        }

        private void OnCurrentChaterCursorMoved(IDataCursor<IComicChapterManager<TResource>> arg1, int arg2)
        {
            IsLoading = true;
            try
            {

                loadCancellationTokenSource?.Cancel();
                loadCancellationTokenSource?.Dispose();
                Resources.Clear();

                var ps = PageSlots;
                if (ps != null)
                {
                    ps.Dispose();
                    PageSlots = null;
                }
                var cpc = CurrentPageCursor;
                if (cpc!=null)
                {
                    cpc.Dispose();
                }
                ps = ChapterSlots[arg2].CreatePageSlots();
                PageSlots = ps;
                var datas = Enumerable.Range(0, PageSlots.Size)
                    .Select(x => CreatePageInfo(ps, x));
                Resources.AddRange(datas);
                CurrentPageCursor = PageSlots.ToDataCursor();
                CurrentChapterWithPage = PageSlots.ChapterManager.ChapterWithPage;
                loadCancellationTokenSource = new CancellationTokenSource();
                OnCurrentChaterCursorChanged(arg1);
                GC.Collect(0, GCCollectionMode.Optimized);
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected virtual ComicPageInfo<TResource> CreatePageInfo(PageSlots<TResource> slots,int index)
        {
            return new ComicPageInfo<TResource>(slots, index);
        }
        public async Task LoadAsync(string address)
        {
            IsLoading = true;
            try
            {
                chapterSlots?.Dispose();
                chapterSlots = null;
                CurrentChaterCursor = null;
                var ok = await Visiting.LoadAsync(address);
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
            return Task.CompletedTask;
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
            return Task.CompletedTask;
        }
        public virtual void Dispose()
        {
            Visiting.Loading -= OnLoading;
            Visiting.Loaded -= OnLoaded;

            chapterSlots?.Dispose();
            pageSlots?.Dispose();
            logoStream?.Dispose();
            Visiting.Dispose();
            scope?.Dispose();
            loadCancellationTokenSource?.Dispose();
        }

        private async Task LoadLogoAsync(string address)
        {
            var r = await OnLoadingLogoAsync(address);
            if (r)
            {
                await OnLoadedLogoAsync(address, false);
                return;
            }
            logoStream?.Dispose();
            if (LogoImage is IDisposable disposable)
            {
                disposable.Dispose();
            }
            using (var rep = await httpClient.GetAsync(address))
            {
                logoStream = recyclableMemoryStreamManager.GetStream();
                await rep.Content.CopyToAsync(logoStream);
                logoStream.Seek(0, SeekOrigin.Begin);
                LogoImage = await streamImageConverter.ToImageAsync(logoStream);
            }
            await OnLoadedLogoAsync(address, true);
        }
        protected virtual Task<bool> OnLoadingLogoAsync(string address)
        {
            return Task.FromResult(false);
        }
        protected virtual Task OnLoadedLogoAsync(string address, bool isDefault)
        {
            return Task.CompletedTask;
        }
    }
}
