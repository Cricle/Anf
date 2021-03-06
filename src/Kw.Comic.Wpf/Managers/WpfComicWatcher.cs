using GalaSoft.MvvmLight.Command;
using Kw.Comic.Engine;
using Kw.Comic.Managers;
using Kw.Comic.PreLoading;
using Kw.Comic.Rendering;
using Kw.Comic.Visit;
using Kw.Comic.Visit.Interceptors;
using Kw.Comic.Wpf.Models;
using Microsoft.Extensions.DependencyInjection;
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
            IComicSourceCondition condition,
            IComicSourceProvider comicSourceProvider)
            : base(comic, condition, comicSourceProvider)
        {
            ServiceScope = serviceScope;

            FirstChapterCommand = new RelayCommand(() => _ = FirstChapterAsync());
            LastChapterCommand = new RelayCommand(() => _ = LastChapterAsync());
            NextChapterCommand = new RelayCommand(() => _ = NextChapterAsync());
            PrevChapterCommand = new RelayCommand(() => _ = PrevChapterAsync());
            NextPageCommand = new RelayCommand(() => _ = NextPageAsync());
            PrevPageCommand = new RelayCommand(() => _ = PrevPageAsync());
            LoadCommand = new RelayCommand(Load);

            PageInfos = new ObservableCollection<WpfComicPageInfo<T>>();
            ComicPageInfos = new ComicPageInfoCollection<T>();
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

        public IServiceScope ServiceScope { get; }

        public ICommand FirstChapterCommand { get; }
        public ICommand LastChapterCommand { get; }
        public ICommand NextChapterCommand { get; }
        public ICommand PrevChapterCommand { get; }
        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand LoadCommand { get; }

        public ComicPageInfoCollection<T> ComicPageInfos { get; }

        public ObservableCollection<WpfComicPageInfo<T>> PageInfos { get; }

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
            var index = -1;

            if (value != null)
            {
                index = PageInfos.IndexOf(value);
            }
            _ = ComicPageInfos.ActiveAsync(index);
            await PageCursor.SetIndexAsync(index);
            if (index >= 0 && index < PageInfos.Count)
            {
                await PageInfos[index].LoadAsync();
            }
            currentPageInfo = value;
            RaisePropertyChanged(nameof(CurrentPageInfo));
            SwitchedPage?.Invoke(this, value);
        }
        protected override void OnPageLoadInterceptorSet(IPageLoadInterceptor<T> interceptor)
        {
            foreach (var item in PageInfos)
            {
                item.Interceptor = interceptor;
            }
        }
        protected override async Task OnLoadChapterAsync(PageCursorBase<T> old, PageCursorBase<T> @new)
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
                var pi = CreatePageInfo(item);
                pi.Interceptor = PageLoadInterceptor;
                PageInfos.Add(pi);
            }
            ComicPageInfos.PageCursor = @new;
            var chapter = ChapterCursor.Current.Chapter;
            #region TEST
            //var sw = Stopwatch.GetTimestamp();
            //foreach (var item in @new.Datas)
            //{
            //    await item.LoadAsync();
            //}
            //foreach (var item in PageInfos)
            //{
            //    await item.LoadAsync();
            //}
            //var ed = Stopwatch.GetTimestamp();
            //MessageBox.Show(new TimeSpan(ed - sw).ToString());
            #endregion
            TotalPage = PageInfos.Count;
            CurrentPageInfo = PageInfos.FirstOrDefault();
            //var start = GC.GetTotalMemory(false) / 1024 / 1024f;
            GC.Collect();
            //MessageBox.Show($"GC after {start-(GC.GetTotalMemory(false) / 1024/1024):f4}M");
            //return Task.CompletedTask;
        }
        protected abstract WpfComicPageInfo<T> CreatePageInfo(T item);
        private void Old_IndexChanged(DataCursor<T> arg1, int arg2)
        {
            var chp = arg1.Current;
            var page = PageInfos.FirstOrDefault(x => x.Visitor == chp);
            CurrentPageInfo = page;
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
