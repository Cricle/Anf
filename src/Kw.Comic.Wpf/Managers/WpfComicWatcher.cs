using GalaSoft.MvvmLight.CommandWpf;
using Kw.Comic.Engine;
using Kw.Comic.Managers;
using Kw.Comic.Visit;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Managers
{
    public class WpfComicWatcher : ComicWatcher, IDisposable
    {
        public WpfComicWatcher(IServiceScope serviceScope,
            ComicEntity comic,
            IHttpClientFactory httpClientFactory,
            IComicSourceCondition condition,
            IComicSourceProvider comicSourceProvider)
            : base(comic, httpClientFactory, condition, comicSourceProvider)
        {
            ServiceScope = serviceScope;

            FirstChapterCommand = new RelayCommand(() => _ = FirstChapterAsync());
            LastChapterCommand = new RelayCommand(() => _ = LastChapterAsync());
            NextChapterCommand = new RelayCommand(() => _ = NextChapterAsync());
            PrevChapterCommand = new RelayCommand(() => _ = PrevChapterAsync());
            NextPageCommand = new RelayCommand(() => _ = NextPageAsync());
            PrevPageCommand = new RelayCommand(() => _ = PrevPageAsync());

            PageCache = new PageCache(10);
            ChapterCacher = new NormalComicChapterCacher<ChapterVisitor>(10);
        }

        private ImageSource comicImage;

        public ImageSource ComicImage
        {
            get { return comicImage; }
            set => RaisePropertyChanged(ref comicImage, value);
        }

        public IServiceScope ServiceScope { get; }

        public ICommand FirstChapterCommand { get; }
        public ICommand LastChapterCommand { get; }
        public ICommand NextChapterCommand { get; }
        public ICommand PrevChapterCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }

        public PageCache PageCache { get; }

        protected override async Task OnLoadChapterAsync(PageCursorBase<ChapterVisitor> old, PageCursorBase<ChapterVisitor> @new)
        {
            if (old != null)
            {
                old.IndexChanged -= Old_IndexChanged;
            }
            if (@new != null)
            {
                @new.IndexChanged += Old_IndexChanged;
            }
            if (@new.Index == -1)
            {
                await @new.LoadIndexAsync(0);
            }
        }
        private BitmapImage Update(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = stream;
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            ComicImage = bitmap;
            return bitmap;
        }
        private void Old_IndexChanged(DataCursor<ChapterVisitor> arg1, int arg2)
        {
            var chp = arg1.Current;
            if (chp != null)
            {
                var pc = PageCache;
                if (pc != null)
                {
                    var cur = new PageCursorIndex(ChapterCursor.Index, arg2);
                    ComicImage = PageCache.GetCache(cur);
                }
                var val = Update(chp.Stream);
                if (pc != null)
                {
                    var cur = new PageCursorIndex(ChapterCursor.Index, arg2);
                    PageCache.SetCache(cur, val);
                }
            }
        }

        public void Dispose()
        {
            ServiceScope?.Dispose();
        }
    }
}
