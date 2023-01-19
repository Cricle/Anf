using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public static class ComicVisitingExtensions
    {
        public static bool IsLoad<TResource>(this IComicVisiting<TResource> visiting)
        {
            if (visiting is null)
            {
                throw new ArgumentNullException(nameof(visiting));
            }

            return !string.IsNullOrEmpty(visiting.Address);
        }
        public static async Task<IComicVisitPage<TResource>> GoToPageAsync<TResource>(this IComicVisiting<TResource> visiting, ComicPos pos)
        {
            if (visiting is null)
            {
                throw new ArgumentNullException(nameof(visiting));
            }

            var mgr = await visiting.GetChapterManagerAsync(pos.ChapterIndex);
            var page = await mgr.GetVisitPageAsync(pos.PageIndex);
            return page;
        }
        public static async Task<IComicVisitPage<TResource>[]> DownloadChapterAsync<TResource>(this IComicVisiting<TResource> visiting,
            int index,
            int concurrent = 1)
        {
            if (visiting is null)
            {
                throw new ArgumentNullException(nameof(visiting));
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

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
