using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public static class ComicVisitingExtensions
    {
        public static async Task<IComicVisitPage> GoPageAsync(this IComicVisiting visiting, ComicPos pos)
        {
            var mgr = await visiting.GetChapterManagerAsync(pos.ChapterIndex);
            var page = await mgr.GetVisitPageAsync(pos.PageIndex);
            return page;
        }
    }
}
