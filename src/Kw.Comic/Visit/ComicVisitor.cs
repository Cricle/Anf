using Kw.Comic.Engine;
using Kw.Core.Input;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public class ComicVisitor :ViewModelBase, IResourceVisitor
    {
        private readonly IComicSourceProvider comicSourceProvider;
        private readonly SemaphoreSlim locker = new SemaphoreSlim(1, 1);
        private bool isLoaded;
        private ChapterWithPage chapterWithPage;

        public ComicVisitor(ComicChapter chapter, ComicEntity comic, IComicSourceProvider comicSourceProvider)
        {
            Comic = comic;
            Chapter = chapter ?? throw new System.ArgumentNullException(nameof(chapter));
            this.comicSourceProvider = comicSourceProvider ?? throw new System.ArgumentNullException(nameof(comicSourceProvider));
        }

        public ChapterWithPage ChapterWithPage 
        {
            get => chapterWithPage;
            private set => RaisePropertyChanged(ref chapterWithPage,value);
        }

        public bool IsLoaded
        {
            get => isLoaded;
            private set => RaisePropertyChanged(ref isLoaded, value);
        }

        public ComicEntity Comic { get; }

        public ComicChapter Chapter { get; }

        public event Action<ComicVisitor, ChapterWithPage> Loading;
        public event Action<ComicVisitor, ChapterWithPage> Loaded;

        public void Dispose()
        {
            locker?.Dispose();
        }

        public async Task LoadAsync(Func<Task<ChapterWithPage>> loadFunc)
        {
            if (loadFunc is null)
            {
                throw new ArgumentNullException(nameof(loadFunc));
            }

            if (IsLoaded)
            {
                return;
            }
            await locker.WaitAsync();
            try
            {
                if (IsLoaded)
                {
                    return;
                }
                Loading?.Invoke(this, ChapterWithPage);
                ChapterWithPage = await loadFunc();
                Loaded?.Invoke(this, ChapterWithPage);
                IsLoaded = true;
            }
            finally
            {
                locker.Release();
            }
        }

        public Task LoadAsync()
        {
            return LoadAsync(DefaultLoadAsync);
        }
        private async Task<ChapterWithPage> DefaultLoadAsync()
        {
            var pages = await comicSourceProvider.GetPagesAsync(Chapter.TargetUrl);
            return new ChapterWithPage(Chapter, pages);
        }
        public override string ToString()
        {
            return $"{{Chapter:{Chapter.Title} Loaded:{IsLoaded}}}";
        }
        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
