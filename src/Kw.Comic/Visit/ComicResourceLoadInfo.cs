namespace Kw.Comic.Visit
{
    public struct ComicResourceLoadInfo<T>
        where T:ChapterVisitorBase
    {
        public readonly ComicWatcherBase<T> Watcher;
        public readonly PageCursorBase<T> PageCursor;
        public readonly T Visitor;
        public readonly ComicVisitor ComicVisitor;

        public ComicResourceLoadInfo(ComicWatcherBase<T> watcher, 
            PageCursorBase<T> pageCursor,
            T visitor,
            ComicVisitor comicVisitor)
        {
            ComicVisitor = comicVisitor;
            Watcher = watcher;
            PageCursor = pageCursor;
            Visitor = visitor;
        }
    }
}
