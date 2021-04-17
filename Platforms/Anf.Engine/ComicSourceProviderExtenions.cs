using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Anf
{
    public static class ComicSourceProviderExtenions
    {
        public static async Task<ComicDetail> GetChapterWithPageAsync(this IComicSourceProvider provider,
            string targetUrl,
            IChapterAnalysisNotifyer notifyer = null,
            CancellationToken token = default(CancellationToken))
        {
            if (notifyer != null)
            {
                await notifyer.FetchingComicAsync(new ComicAnalysingContext { Address = targetUrl, Provider = provider });
            }
            var cap = await provider.GetChaptersAsync(targetUrl);
            if (notifyer != null)
            {
                await notifyer.FetchedComicAsync(new ComicAnalysedContext { Entity = cap, Address = targetUrl, Provider = provider });
            }
            token.ThrowIfCancellationRequested();
            var cwps = new List<ChapterWithPage>();
            for (int a = 0; a < cap.Chapters.Length; a++)
            {
                token.ThrowIfCancellationRequested();
                var c = cap.Chapters[a];
                if (notifyer != null)
                {
                    await notifyer.FetchingChapterAsync(new ChapterAnalysingContext { Chapter = c, Index = a, Entity = cap, Address = targetUrl, Provider = provider });
                }
                var pages = await provider.GetPagesAsync(c.TargetUrl);
                var cwp = new ChapterWithPage(c, pages);
                cwps.Add(cwp);
                if (notifyer != null)
                {
                    await notifyer.FetchedChapterAsync(new ChapterAnalysedContext { ChapterWithPage = cwp, Chapter = c, Index = a, Entity = cap, Address = targetUrl, Provider = provider });
                }
            }
            return new ComicDetail
            {
                Entity = cap,
                Chapters = cwps.ToArray()
            };
        }
    }
}
