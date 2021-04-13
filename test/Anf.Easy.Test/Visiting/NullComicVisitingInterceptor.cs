using Anf.Easy.Visiting;
using System.Threading.Tasks;

namespace Anf.Easy.Test.Visiting
{
    internal class NullComicVisitingInterceptor<T> : IComicVisitingInterceptor<T>
    {
        public bool IsGettingPageAsync { get; set; }
        public bool IsGotChapterManagerAsync { get; set; }
        public bool IsGotPageAsync { get; set; }
        public bool IsLoadedChapterAsync { get; set; }
        public bool IsLoadedComicAsync { get; set; }
        public bool IsLoadingChapterAsync { get; set; }
        public bool IsLoadingComicAsync { get; set; }
        public Task GettingPageAsync(GettingPageInterceptorContext<T> context)
        {
            IsGettingPageAsync = true;
            return Task.FromResult(0);
        }

        public Task GotChapterManagerAsync(GotChapterManagerInterceptorContext<T> context)
        {
            IsGotChapterManagerAsync = true;
            return Task.FromResult(0);
        }

        public Task GotPageAsync(GettingPageInterceptorContext<T> context)
        {
            IsGotPageAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadedChapterAsync(ChapterVisitingInterceptorContext<T> context)
        {
            IsLoadedChapterAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadedComicAsync(LoadVisitingInterceptorContext<T> context)
        {
            IsLoadedComicAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadingChapterAsync(ChapteringVisitingInterceptorContext<T> context)
        {
            IsLoadingChapterAsync = true;
            return Task.FromResult(0);
        }

        public Task LoadingComicAsync(LoadVisitingInterceptorContext<T> context)
        {
            IsLoadingComicAsync = true;
            return Task.FromResult(0);
        }
    }
}
