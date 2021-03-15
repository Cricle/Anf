using System;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public class DelegateComicSaver : IComicSaver
    {
        public DelegateComicSaver(Func<ResolvedChapterContext, Task> resolvedChapter, Func<ComicSaveContext, Task> save)
        {
            ResolvedChapter = resolvedChapter;
            Save = save;
        }

        public Func<ResolvedChapterContext, Task> ResolvedChapter { get; }
        public Func<ComicSaveContext, Task> Save { get; }

        public Task ResolvedChapterAsync(ResolvedChapterContext context)
        {
            if (ResolvedChapter != null)
            {
                return ResolvedChapter(context);
            }
#if NET45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }

        public Task SaveAsync(ComicSaveContext context)
        {
            if (Save!=null)
            {
                return Save(context);
            }
#if NET45
            return Task.FromResult(0);
#else
            return Task.CompletedTask;
#endif
        }
    }
}
