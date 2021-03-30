using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kw.Comic.ViewModels
{
    public class VisitingViewModel<TResource,TImage> : ViewModelBase, IDisposable
    {
        public VisitingViewModel(IComicVisiting<TResource> visiting=null)
        {
            scope = AppEngine.CreateScope();
            InitService(scope.ServiceProvider, visiting);
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
            PrevChapterCommand = new RelayCommand(() => _ = PrevChapterAsync());
            NextChapterCommand = new RelayCommand(() => _ = NextChapterAsync());
            GoChapterCommand = new RelayCommand<ComicChapter>(x => _ = GoChapterAsync(x));

            PrevPageCommand = new RelayCommand(() => _ = PrevPageAsync());
            NextPageCommand = new RelayCommand(() => _ = NextPageAsync());
            GoPageCommand = new RelayCommand<ComicPage>(x => _ = GoPageAsync(x));

            if (Visiting.IsLoad())
            {
                Init();
            }
        }
        private IComicVisiting<TResource> visiting;
        protected IServiceScope scope;
        protected IStreamImageConverter<TImage> streamImageConverter;
        protected RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        protected HttpClient httpClient;
        private MemoryStream logoStream;
        private bool isLoading;
        private ChapterSlots<TResource> chapterSlots;
        private PageSlots<TResource> pageSlots;
        private IDataCursor<IComicChapterManager<TResource>> currentChaterCursor;
        private IDataCursor<IComicVisitPage<TResource>> currentPageCursor;
        private TImage logoImage;

        public MemoryStream LogoStream => logoStream;

        public TImage LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }


        public IDataCursor<IComicVisitPage<TResource>> CurrentPageCursor
        {
            get { return currentPageCursor; }
            private set => Set(ref currentPageCursor, value);
        }

        public IDataCursor<IComicChapterManager<TResource>> CurrentChaterCursor
        {
            get { return currentChaterCursor; }
            private set => Set(ref currentChaterCursor, value);
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set => Set(ref isLoading, value);
        }

        public IComicVisiting<TResource> Visiting => visiting;

        public ComicEntity ComicEntity => Visiting.Entity;

        public RelayCommand PrevChapterCommand { get; }
        public RelayCommand NextChapterCommand { get; }
        public RelayCommand<ComicChapter> GoChapterCommand { get; }

        public RelayCommand PrevPageCommand { get; }
        public RelayCommand NextPageCommand { get; }
        public RelayCommand<ComicPage> GoPageCommand { get; }

        protected void InitService(IServiceProvider provider, IComicVisiting<TResource> visiting=null)
        {
            this.visiting = visiting ?? provider.GetRequiredService<IComicVisiting<TResource>>();
            httpClient = provider.GetRequiredService<HttpClient>();
            recyclableMemoryStreamManager = provider.GetRequiredService<RecyclableMemoryStreamManager>();
            streamImageConverter = provider.GetRequiredService<IStreamImageConverter<TImage>>();
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
        public Task<bool> GoPageAsync(ComicPage page)
        {
            var chCur = CurrentChaterCursor?.Current;
            var cur = CurrentPageCursor;
            if (cur != null && chCur != null)
            {
                var index = Array.FindIndex(chCur.ChapterWithPage.Pages, x => x == page);
                return cur.MoveAsync(index);
            }
            return Task.FromResult(false);
        }
        public Task<bool> GoChapterAsync(ComicChapter chapter)
        {
            var cur = CurrentChaterCursor;
            if (cur != null)
            {
                var index = Array.FindIndex(Visiting.Entity.Chapters, x => x == chapter);
                return cur.MoveAsync(index);
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
        private void Init()
        {
            chapterSlots = Visiting.CreateChapterSlots();
            CurrentChaterCursor = chapterSlots.ToDataCursor();
            _ = LoadLogoAsync(Visiting.Entity.ImageUrl);
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
                    Init();
                }
            }
            finally
            {
                IsLoading = false;
            }
        }

        public async Task SelectChapterAsync(int index)
        {
            pageSlots?.Dispose();
            pageSlots = null;
            var cur = CurrentChaterCursor;
            if (cur != null && cur.CurrentIndex != index)
            {
                var ok = await cur.MoveAsync(index);
                if (ok)
                {
                    pageSlots = cur.Current.CreatePageSlots();
                    CurrentPageCursor = pageSlots.ToDataCursor();
                }
            }
        }

        public virtual void Dispose()
        {
            chapterSlots?.Dispose();
            pageSlots?.Dispose();
            logoStream?.Dispose();
            Visiting.Dispose();
            scope?.Dispose();
        }

        private async Task LoadLogoAsync(string address)
        {
            logoStream?.Dispose();
            using (var rep = await httpClient.GetAsync(address)) 
            {
                logoStream = recyclableMemoryStreamManager.GetStream();
                await rep.Content.CopyToAsync(logoStream);
                logoStream.Seek(0, SeekOrigin.Begin);
                LogoImage=await streamImageConverter.ToImageAsync(logoStream);
            }
        }
    }
}
