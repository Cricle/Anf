using System.Threading.Tasks;

namespace Anf
{
    public interface IChapterAnalysisNotifyer
    {
        Task FetchingComicAsync(ComicAnalysingContext context);
        Task FetchedComicAsync(ComicAnalysedContext context);
        Task FetchingChapterAsync(ChapterAnalysingContext context);
        Task FetchedChapterAsync(ChapterAnalysedContext context);
    }
}
