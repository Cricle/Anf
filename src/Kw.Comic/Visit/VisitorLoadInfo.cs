using System;
using System.Diagnostics;

namespace Kw.Comic.Visit
{
    public class DebugVisitorLoadInfo<T> : VisitorLoadInfo<T>
        where T : ChapterVisitorBase
    {
        public DebugVisitorLoadInfo(ComicWatcherBase<T> watcher) : base(watcher)
        {
        }
        protected override void OnVisitorLoaded(ComicResourceLoadInfo<T> info)
        {
            Debug.WriteLine($"{info.ComicVisitor.Chapter.Title} - {info.Visitor.Page.Name}","PageLoaded!");
        }
    }
    public abstract class VisitorLoadInfo<T> : IDisposable
        where T: ChapterVisitorBase
    {
        protected readonly ComicWatcherBase<T> watcher;

        public VisitorLoadInfo(ComicWatcherBase<T> watcher)
        {
            this.watcher = watcher;
            watcher.VisitorLoaded += OnVisitorLoaded;
        }

        public void Dispose()
        {
            watcher.VisitorLoaded -= OnVisitorLoaded;
            OnDispose();
        }
        protected virtual void OnDispose()
        {

        }

        protected virtual void OnVisitorLoaded(ComicResourceLoadInfo<T> info)
        {
        }
    }
}
