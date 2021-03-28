using GalaSoft.MvvmLight;
using Kw.Comic.Engine.Easy.Visiting;
using Microsoft.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Kw.Comic.ViewModels
{
    public class VisitingViewModel : ViewModelBase, IDisposable
    {
        public VisitingViewModel(
            IComicVisiting<ImageSource> visiting,
            HttpClient httpClient,
            RecyclableMemoryStreamManager recyclableMemoryStreamManager)
        {
            this.recyclableMemoryStreamManager = recyclableMemoryStreamManager;
            this.httpClient = httpClient;
            Visiting = visiting;
            PrevChapterCommand = new Command(() => _ = PrevChapterAsync());
            NextChapterCommand = new Command(() => _ = NextChapterAsync());
            GoChapterCommand = new Command<ComicChapter>(x => _ = GoChapterAsync(x));

            PrevPageCommand = new Command(() => _ = PrevPageAsync());
            NextPageCommand = new Command(() => _ = NextPageAsync());
            GoPageCommand = new Command<ComicPage>(x => _ = GoPageAsync(x));
        }
        private readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        private readonly HttpClient httpClient;
        private MemoryStream logoStream;
        private bool isLoading;
        private ChapterSlots<ImageSource> chapterSlots;
        private PageSlots<ImageSource> pageSlots;
        private IDataCursor<IComicChapterManager<ImageSource>> currentChaterCursor;
        private IDataCursor<IComicVisitPage<ImageSource>> currentPageCursor;
        private ImageSource logoImage;

        public ImageSource LogoImage
        {
            get { return logoImage; }
            private set => Set(ref logoImage, value);
        }


        public IDataCursor<IComicVisitPage<ImageSource>> CurrentPageCursor
        {
            get { return currentPageCursor; }
            private set => Set(ref currentPageCursor, value);
        }

        public IDataCursor<IComicChapterManager<ImageSource>> CurrentChaterCursor
        {
            get { return currentChaterCursor; }
            private set => Set(ref currentChaterCursor, value);
        }

        public bool IsLoading
        {
            get { return isLoading; }
            private set => Set(ref isLoading, value);
        }

        public IComicVisiting<ImageSource> Visiting { get; }

        public Command PrevChapterCommand { get; }
        public Command NextChapterCommand { get; }
        public Command<ComicChapter> GoChapterCommand { get; }

        public Command PrevPageCommand { get; }
        public Command NextPageCommand { get; }
        public Command<ComicPage> GoPageCommand { get; }

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
                    chapterSlots = Visiting.CreateChapterSlots();
                    CurrentChaterCursor = chapterSlots.ToDataCursor();
                    _ = LoadLogoAsync(Visiting.Entity.ImageUrl);
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

        public void Dispose()
        {
            chapterSlots?.Dispose();
            pageSlots?.Dispose();
            logoStream?.Dispose();
            Visiting.Dispose();
        }

        private async Task LoadLogoAsync(string address)
        {
            logoStream?.Dispose();
            using (var rep = await httpClient.GetAsync(address)) 
            {
                logoStream = recyclableMemoryStreamManager.GetStream();
                await rep.Content.CopyToAsync(logoStream);
                logoStream.Seek(0, SeekOrigin.Begin);
                LogoImage = ImageSource.FromStream(() => logoStream);
            }
        }
    }
}
