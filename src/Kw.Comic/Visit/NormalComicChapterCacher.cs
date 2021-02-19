namespace Kw.Comic.Visit
{
    public class NormalComicChapterCacher<T> : SimpleCacher<int, PageCursorBase<T>>, IComicChapterCacher<T>
        where T : ChapterVisitorBase
    {
        public NormalComicChapterCacher(int max = 5)
            : base(max)
        {
        }
    }
}
