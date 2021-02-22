using GalaSoft.MvvmLight.CommandWpf;
using Kw.Comic.Engine;
using Kw.Comic.Managers;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

            ChapterCacher = new NormalComicChapterCacher<ChapterVisitor>(10);
            PageInfos = new ObservableCollection<ComicPageInfo>();
        }

        private ComicPageInfo currentPageInfo;
        private int pageIndex;
        private int totalPage;

        public int TotalPage
        {
            get { return totalPage; }
            private set => RaisePropertyChanged(ref totalPage, value);
        }

        public int PageIndex
        {
            get { return pageIndex; }
            private set => RaisePropertyChanged(ref pageIndex, value);
        }

        public ComicPageInfo CurrentPageInfo
        {
            get { return currentPageInfo; }
            set
            {
                RaisePropertyChanged(ref currentPageInfo, value);
                if (value != null)
                {
                    _ = value.LoadAsync();
                    PageIndex = PageInfos.IndexOf(value);
                    if (PageIndex != -1)
                    {
                        var preLoadCount = Math.Max(0, PreLoadCount);
                        for (int i = PageIndex; i < PageInfos.Count && preLoadCount > 0; i++)
                        {
                            _ = PageInfos[i].LoadAsync();
                            preLoadCount--;
                        }
                    }
                }
            }
        }

        public IServiceScope ServiceScope { get; }

        public ICommand FirstChapterCommand { get; }
        public ICommand LastChapterCommand { get; }
        public ICommand NextChapterCommand { get; }
        public ICommand PrevChapterCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }

        public int PreLoadCount { get; set; } = 3;


        public ObservableCollection<ComicPageInfo> PageInfos { get; }

        protected override Task OnLoadChapterAsync(PageCursorBase<ChapterVisitor> old, PageCursorBase<ChapterVisitor> @new)
        {
            if (old != null)
            {
                old.IndexChanged -= Old_IndexChanged;
            }
            if (@new != null)
            {
                @new.IndexChanged += Old_IndexChanged;
            }
            CurrentPageInfo = null;
            PageInfos.Clear();
            foreach (var item in @new.Datas)
            {
                PageInfos.Add(new ComicPageInfo(item));
            }
            TotalPage = PageInfos.Count;
            CurrentPageInfo = PageInfos.FirstOrDefault();
            return Task.CompletedTask;
        }
        private void Old_IndexChanged(DataCursor<ChapterVisitor> arg1, int arg2)
        {
            var chp = arg1.Current;
            CurrentPageInfo = PageInfos.FirstOrDefault(x => x.Visitor == chp);
        }
        public void Dispose()
        {
            ServiceScope?.Dispose();
        }
    }
}
