using System;
using System.Threading.Tasks;

namespace Anf.Easy.Visiting
{
    public class ComicChapterManager<TResource> : IComicChapterManager<TResource>
    {
        public ComicChapterManager(ChapterWithPage chapterWithPage,
            ComicVisiting<TResource> comicVisiting)
        {
            ComicVisiting = comicVisiting ?? throw new System.ArgumentNullException(nameof(comicVisiting));
            ChapterWithPage = chapterWithPage ?? throw new System.ArgumentNullException(nameof(chapterWithPage));
        }

        public ComicVisiting<TResource> ComicVisiting { get; }

        public ChapterWithPage ChapterWithPage { get; }

        public async Task<IComicVisitPage<TResource>> GetVisitPageAsync(int index)
        {
            if (ComicVisiting.ResourceFactory is null)
            {
                throw new InvalidOperationException("The ComicVisiting.ResourceFactory is null, can't do that");
            }
            var page = ChapterWithPage.Pages[index];
            var inter = ComicVisiting.VisitingInterceptor;
            GettingPageInterceptorContext<TResource> context = null;
            if (inter != null)
            {
                context = new GettingPageInterceptorContext<TResource>
                {
                    Index = index,
                    ChapterManager = this,
                    Page = page,
                    Visiting = ComicVisiting
                };
                await inter.GettingPageAsync(context);
            }
            var s = await ComicVisiting.ResourceFactory.GetAsync(page.TargetUrl);
            if (inter != null)
            {
                await inter.GotPageAsync(context);
            }
            return new PageBox<TResource> { Page = page, Resource = s };
        }
    }

}
