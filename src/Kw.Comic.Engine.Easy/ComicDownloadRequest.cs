namespace Kw.Comic.Engine.Easy
{
    public class ComicDownloadRequest
    {
        public ComicDownloadRequest(IComicSaver saver, ComicDetail detail, IComicSourceProvider provider)
        {
            Saver = saver;
            Detail = detail;
            Provider = provider;
        }

        public IComicSaver Saver { get; }

        public ComicDetail Detail { get; }

        public IComicSourceProvider Provider { get; }
    }
}
