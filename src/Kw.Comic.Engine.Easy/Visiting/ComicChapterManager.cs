using System.IO;
using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    internal class ComicChapterManager : IComicChapterManager
    {
        class PageBox : IComicVisitPage
        {
            public ComicPage Page { get; set; }

            public Stream Stream { get; set; }

        }
        private readonly ISharedComic sharedComic;

        public ComicChapterManager(ISharedComic sharedComic, ChapterWithPage chapterWithPage)
        {
            this.sharedComic = sharedComic;
            ChapterWithPage = chapterWithPage;
        }

        public ChapterWithPage ChapterWithPage { get; }

        public async Task<IComicVisitPage> GetVisitPageAsync(int index)
        {
            var page = ChapterWithPage.Pages[index];
            var s = await sharedComic.GetOrLoadAsync(page.TargetUrl);
            return new PageBox { Page = page, Stream = s };
        }
    }
}
