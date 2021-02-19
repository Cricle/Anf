namespace Kw.Comic.Visit
{
    public interface IComicChapterCacher<T> : ISimpleCacher<int,PageCursorBase<T>>
        where T: ChapterVisitorBase
    {
    }
}
