namespace Kw.Comic.Engine
{
    public class ChapterAnalysingContext : ComicAnalysedContext
    {
        public int Index { get; internal set; }

        public ComicChapter Chapter { get; internal set; }
    }
}
