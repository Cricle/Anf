using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Managers;
using Kw.Comic.PreLoading;
using Kw.Comic.Visit;
using Kw.Comic.Wpf.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
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
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Kw.Comic.Wpf.Managers
{
    public abstract class WpfComicWatcher<T> : ComicWatcherBase<T>, IDisposable
         where T : ChapterVisitorBase
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
            LoadCommand = new RelayCommand(Load);

            //ChapterCacher = new NormalComicChapterCacher<SkiaChapterVisitor>(1);
            PageInfos = new ComicPageInfoCollection<WpfComicPageInfo<T>,T,ImageSource>();
        }
        private WpfComicPageInfo<T> currentPageInfo;
        private int totalPage;
        private int currentIndex;

        public int CurrentIndex
        {
            get { return currentIndex; }
            set
            {
                RaisePropertyChanged(ref currentIndex, value);
                CurrentPageInfo = PageInfos[value];
            }
        }

        public int TotalPage
        {
            get { return totalPage; }
            private set => RaisePropertyChanged(ref totalPage, value);
        }

        public WpfComicPageInfo<T> CurrentPageInfo
        {
            get { return currentPageInfo; }
            set
            {
                if (currentPageInfo != value)
                {
                    currentPageInfo = value;
                    OnSwitch(value);
                }
            }
        }
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        public IServiceScope ServiceScope { get; }

        public ICommand FirstChapterCommand { get; }
        public ICommand LastChapterCommand { get; }
        public ICommand NextChapterCommand { get; }
        public ICommand PrevChapterCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand LoadCommand { get; }

        public ComicPageInfoCollection<WpfComicPageInfo<T>, T, ImageSource> PageInfos { get; }

        public event Action<WpfComicWatcher<T>, WpfComicPageInfo<T>> SwitchedPage;

        private async void Load()
        {
            if (CurrentPageInfo != null)
            {
                await CurrentPageInfo.UnLoadAsync();
                await CurrentPageInfo.LoadAsync();
            }
        }

        private async void OnSwitch(WpfComicPageInfo<T> value)
        {
            //await semaphoreSlim.WaitAsync();
            try
            {
                var index = -1;

                if (value != null)
                {
                    index = PageInfos.IndexOf(value);
                }
                _= PageInfos.ActiveAsync(index);
                await PageCursor.SetIndexAsync(index);
                currentPageInfo = value;
                RaisePropertyChanged(nameof(CurrentPageInfo));
                SwitchedPage?.Invoke(this, value);
            }
            finally
            {
                //semaphoreSlim.Release();
            }
        }

        protected override Task OnLoadChapterAsync(PageCursorBase<T> old, PageCursorBase<T> @new)
        {
            if (old != null)
            {
                old.IndexChanged -= Old_IndexChanged;
                old.Dispose();
            }
            if (@new != null)
            {
                @new.IndexChanged += Old_IndexChanged;
            }
            CurrentPageInfo = null;
            foreach (var item in PageInfos)
            {
                item.Dispose();
            }
            PageInfos.Clear();
            foreach (var item in @new.Datas)
            {
                PageInfos.Add(CreatePageInfo(item));
            }
            TotalPage = PageInfos.Count;
            CurrentPageInfo = PageInfos.FirstOrDefault();
            GC.Collect();
            return Task.CompletedTask;
        }
        protected abstract WpfComicPageInfo<T> CreatePageInfo(T item);
        private void Old_IndexChanged(DataCursor<T> arg1, int arg2)
        {
            var chp = arg1.Current;
            CurrentPageInfo = PageInfos.FirstOrDefault(x => x.Visitor == chp);
        }
        public void Dispose()
        {
            ServiceScope?.Dispose();
            foreach (var item in PageInfos)
            {
                item.Dispose();
            }
        }
    }
}
