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
using System.Threading.Tasks;
using System.Windows.Input;

namespace Kw.Comic.Render
{
    public abstract class ViewViewModelBase<TResource,TImage>:ViewModelBase
    {
        public ViewViewModelBase(IServiceScope scope, IComicVisiting<TResource> visiting)
        {
            if (!visiting.IsLoad())
            {
                throw new InvalidOperationException("Does't accept not loaded visiting!");
            }
            this.scope = scope;
            recyclableMemoryStreamManager = scope.ServiceProvider.GetRequiredService<RecyclableMemoryStreamManager>();
            ComicEntity = visiting.Entity;
            chapterSlots = visiting.CreateChapterSlots();

            NextChapterCommand = new RelayCommand(NextChapter);
            PrevChapterCommand = new RelayCommand(PrevChapter);
            GoChapterCommand = new RelayCommand<ComicChapter>(GoChapter);
            NextPageCommand = new RelayCommand(NextPage);
            PrevPageCommand = new RelayCommand(PrevPage);
            GoPageCommand = new RelayCommand<ComicPage>(GoPage);

        }
        private IComicChapterManager<TResource> currentComicVisitor;
        protected readonly RecyclableMemoryStreamManager recyclableMemoryStreamManager;
        protected readonly ChapterSlots<TResource> chapterSlots;
        protected readonly IServiceScope scope;
        private TImage converImage;
        private int chapterIndex;

        private int currentIndex;
        private IComicVisitPage<TResource> currentPage;
        private PageSlots<TResource> pageSlots;

        public PageSlots<TResource> PageSlots
        {
            get { return pageSlots; }
            private set => Set(ref pageSlots, value);
        }

        public IComicVisitPage<TResource> CurrentPage
        {
            get { return currentPage; }
            private set => Set(ref currentPage, value);
        }

        public int CurrentIndex
        {
            get { return currentIndex; }
            set => Set(ref currentIndex, value);
        }
        public IComicChapterManager<TResource> CurrentComicVisitor
        {
            get { return currentComicVisitor; }
            private set => Set(ref currentComicVisitor, value);
        }

        public TImage ConverImage
        {
            get { return converImage; }
            protected set => Set(ref converImage, value);
        }

        public int ChapterIndex
        {
            get => chapterIndex;
            private set
            {
                Set(ref chapterIndex, value);
            }
        }

        public IComicVisiting<TResource> Visiting { get; }

        public ComicEntity ComicEntity { get; }


        public ICommand NextPageCommand { get; }
        public ICommand PrevPageCommand { get; }
        public ICommand GoChapterCommand { get; }
        public ICommand GoPageCommand { get; }
        public ICommand NextChapterCommand { get; }
        public ICommand PrevChapterCommand { get; }

        public bool CheckChapterIndex(int index)
        {
            if (chapterSlots.Size == 0)
            {
                return false;
            }
            return index >= 0 && index < chapterSlots.Size;
        }
        public bool CheckPageIndex(int index)
        {
            var slot = PageSlots;
            if (slot == null || slot.Size == 0)
            {
                return false;
            }
            return index >= 0 && index < slot.Size;
        }
        public void GoPage(ComicPage page)
        {
            var visitor = CurrentComicVisitor;
            if (visitor != null)
            {
                var index = Array.FindIndex(visitor.ChapterWithPage.Pages, x => x == page);
                GoPage(index);
            }
        }
        public async void GoPage(int pageIndex)
        {
            if (!CheckPageIndex(pageIndex))
            {
                return;
            }
            var slots = PageSlots;
            if (slots != null)
            {
                var old = currentPage;
                var page = await slots.GetAsync(pageIndex);
                if (page == old)
                {
                    CurrentPage = page;
                    CurrentIndex = pageIndex;
                }
            }
        }
        public void PrevPage()
        {
            GoPage(CurrentIndex - 1);
        }
        public void NextPage()
        {
            GoPage(CurrentIndex + 1);
        }
        public void GoChapter(ComicChapter chapter)
        {
            var index = Array.FindIndex(Visiting.Entity.Chapters, x => x == chapter);
            GoChapter(chapter);
        }
        public async void GoChapter(int index)
        {
            if (!CheckChapterIndex(index))
            {
                return;
            }
            ChapterIndex = index;
            var old = CurrentComicVisitor;
            var visitor = await chapterSlots.GetAsync(index);
            if (old != CurrentComicVisitor)
            {
                PageSlots = CurrentComicVisitor.CreatePageSlots();
                ChapterIndex = index;
                CurrentComicVisitor = visitor;
                OnGoChapter(index, visitor);
            }
        }
        protected virtual void OnGoChapter(int index,IComicChapterManager<TResource> chapterManager)
        {

        }
        public void NextChapter()
        {
            GoChapter(ChapterIndex + 1);
        }
        public void PrevChapter()
        {
            GoChapter(ChapterIndex - 1);
        }


        public virtual void Dispose()
        {
            scope.Dispose();
            Visiting.Dispose();
        }
    }
}
