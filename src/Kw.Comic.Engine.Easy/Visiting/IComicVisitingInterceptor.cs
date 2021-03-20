using System.Threading.Tasks;

namespace Kw.Comic.Engine.Easy.Visiting
{
    public interface IComicVisitingInterceptor<TResource>
    {
        Task LoadingComicAsync(LoadVisitingInterceptorContext<TResource> context);

        Task LoadedComicAsync(LoadVisitingInterceptorContext<TResource> context);

        Task LoadingChapterAsync(ChapteringVisitingInterceptorContext<TResource> context);

        Task LoadedChapterAsync(ChapterVisitingInterceptorContext<TResource> context);

        Task GotChapterManagerAsync(GotChapterManagerInterceptorContext<TResource> context);

        Task GettingPageAsync(GettingPageInterceptorContext<TResource> context);

        Task GotPageAsync(GettingPageInterceptorContext<TResource> context);
    }
}
