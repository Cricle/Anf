namespace Anf.Easy
{
    public readonly struct DownloadLink
    {
        public readonly IComicSourceProviderHost Host;
        public readonly IComicDownloader Downloader;
        public readonly ComicDownloadRequest Request;

        public DownloadLink(IComicSourceProviderHost host, IComicDownloader downloader, ComicDownloadRequest request)
        {
            Host = host;
            Downloader = downloader;
            Request = request;
        }
    }
}
