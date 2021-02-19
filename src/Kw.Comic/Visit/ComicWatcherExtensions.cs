using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Kw.Comic.Visit
{
    public static class ComicWatcherExtensions
    {
        public static async Task<bool> DownloadAllAsync<T>(this ComicWatcherBase<T> watcher,DownloadAllOptions options, CancellationToken token = default)
            where T : ChapterVisitorBase
        {
            if (options.Parallel && options.ParallelThread <= 0)
            {
                throw new ArgumentException("In Parallel, ParallelThread must more than zero!");
            }
            try
            {
                var saver = options.ComicSaver;
                var solts = new PageCursorBase<T>[watcher.ChapterCursor.Length];
                for (int i = 0; i < watcher.ChapterCursor.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var chapter = watcher.ChapterCursor[i];
                    var pageCursor = await watcher.CoreLoadChapterAsync(i, false, options.CachePageCursor);
                    solts[i] = pageCursor;
                    if (saver != null)
                    {
                        var ctx = new ResolvedChapterContext(watcher.ChapterCursor.Length - 1, i, chapter, watcher.Comic);
                        await saver.ResolvedChapterAsync(ctx);
                    }
                }
                var total = solts.Sum(x => x.Length);
                var current = 0;
                var idx = 0;
                var tasks = new Func<Task>[total];
                for (int i = 0; i < solts.Length; i++)
                {
                    token.ThrowIfCancellationRequested();
                    var chapter = watcher.ChapterCursor[i];
                    var solt = solts[i];
                    for (int j = 0; j < solt.Length; j++)
                    {
                        var x = j;
                        tasks[idx++] = async () =>
                        {
                            token.ThrowIfCancellationRequested();
                            await solt.LoadIndexAsync(x);
                            var pageVisitor = solt[x];
                            var ctx = new ComicSaveContext(total - 1, current, chapter.ChapterWithPage,
                                watcher.Comic, pageVisitor);
                            if (saver != null)
                            {
                                await saver.SaveAsync(ctx);
                            }
                            Interlocked.Increment(ref current);
                        };
                    }
                }
                if (options.Parallel)
                {
                    var quene = new Queue<Func<Task>>(tasks);
                    var task = new List<Task>(quene.Take(options.ParallelThread)
                        .Select(x => x()));
                    while (quene.Count > 0 || task.Count > 0)
                    {
                        token.ThrowIfCancellationRequested();
                        if (task.Count < options.ParallelThread && quene.Count > 0)
                        {
                            task.Add(quene.Dequeue()());
                        }
                        var tsk = await Task.WhenAny(task);
                        task.Remove(tsk);
                    }
                    await Task.WhenAll(task);
                }
                else
                {
                    foreach (var item in tasks)
                    {
                        token.ThrowIfCancellationRequested();
                        await item();
                    }
                }
                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
        }
    }
}
