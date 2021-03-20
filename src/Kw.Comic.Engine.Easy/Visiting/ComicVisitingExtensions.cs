using Microsoft.Extensions.DependencyInjection;
using Microsoft.IO;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public static class ComicVisitingExtensions
    {
        public static async Task<IComicVisitPage<TResource>> GoToPageAsync<TResource>(this IComicVisiting<TResource> visiting, ComicPos pos)
        {
            var mgr = await visiting.GetChapterManagerAsync(pos.ChapterIndex);
            var page = await mgr.GetVisitPageAsync(pos.PageIndex);
            return page;
        }
        public static async Task<IComicVisitPage<TResource>[]> DownloadChapterAsync<TResource>(this IComicVisiting<TResource> visiting,
            int index,
            int concurrent = 1)
        {
            var streamMgr = visiting.Host.GetRequiredService<RecyclableMemoryStreamManager>();
            var mgr = await visiting.GetChapterManagerAsync(index);
            if (mgr == null || mgr.ChapterWithPage?.Pages == null)
            {
                return null;
            }
            var workingTask = Enumerable.Range(0, mgr.ChapterWithPage.Pages.Length)
                .Select(x => new Func<Task<IComicVisitPage<TResource>>>(() => mgr.GetVisitPageAsync(x)))
                .ToArray();
            var orderMap = Enumerable.Range(0, mgr.ChapterWithPage.Pages.Length)
                .ToDictionary(x => mgr.ChapterWithPage.Pages[x]);
            var results = await TaskQuene.RunAsync(workingTask, concurrent);
            return results.OrderBy(x => orderMap[x.Page]).ToArray();
        }
    }
}
