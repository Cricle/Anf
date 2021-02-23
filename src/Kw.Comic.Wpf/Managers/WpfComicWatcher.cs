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
using System.Threading;
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
            LoadCommand = new RelayCommand(Load);

            ChapterCacher = new NormalComicChapterCacher<ChapterVisitor>(2);
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
            set
            {
                if (pageIndex != value)
                {
                    RaisePropertyChanged(ref pageIndex, value);
                    var val = PageInfos[value];
                    if (CurrentPageInfo != val)
                    {
                        CurrentPageInfo = val;
                    }
                }
            }
        }

        public ComicPageInfo CurrentPageInfo
        {
            get { return currentPageInfo; }
            set
            {
                OnSwitch(value);                
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

        public ObservableCollection<ComicPageInfo> PageInfos { get; }

        private async void Load()
        {
            if (CurrentPageInfo!=null)
            {
                await CurrentPageInfo.UnLoadAsync();
                await CurrentPageInfo.LoadAsync();
            }
        }

        private async void OnSwitch(ComicPageInfo value)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (currentPageInfo != null)
                {
                    await currentPageInfo.UnLoadAsync();
                }
                if (value != null)
                {
                    PageIndex = PageInfos.IndexOf(value);
                    if (PageIndex == -1)
                    {
                        return;
                    }
                    await value.LoadAsync();
                }
                currentPageInfo = value;
                RaisePropertyChanged(nameof(CurrentPageInfo));
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

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
            foreach (var item in PageInfos)
            {
                item.Dispose();
            }
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
            foreach (var item in PageInfos)
            {
                item.Dispose();
            }
        }
    }
}
