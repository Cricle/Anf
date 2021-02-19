using Kw.Comic.Engine;
using Kw.Core.Input;
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

        public ComicVisitor(ComicChapter chapter, IComicSourceProvider comicSourceProvider)
        {
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

        public ComicChapter Chapter { get; }

        public void Dispose()
        {
            locker?.Dispose();
        }

        public async Task LoadAsync()
        {
            if (IsLoaded)
            {
                return;
            }
            await locker.WaitAsync();
            if (IsLoaded)
            {
                return;
            }
            try
            {
                var pages = await comicSourceProvider.GetPagesAsync(Chapter.TargetUrl);
                ChapterWithPage = new ChapterWithPage(Chapter, pages);
                IsLoaded = true;
            }
            finally
            {
                locker.Release();
            }
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
