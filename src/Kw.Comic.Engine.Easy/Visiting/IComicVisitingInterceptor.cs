using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicVisitingInterceptor
    {
        Task LoadingComicAsync(LoadVisitingInterceptorContext context);

        Task LoadedComicAsync(LoadVisitingInterceptorContext context);

        Task LoadingChapterAsync(ChapteringVisitingInterceptorContext context);

        Task LoadedChapterAsync(ChapterVisitingInterceptorContext context);

        Task GotChapterManagerAsync(GotChapterManagerInterceptorContext context);

        Task GettingPageAsync(GettingPageInterceptorContext context);

        Task GotPageAsync(GettingPageInterceptorContext context);
    }
}
