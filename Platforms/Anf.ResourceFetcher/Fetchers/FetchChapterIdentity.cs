namespace Anf.ResourceFetcher.Fetchers
{
    public readonly struct FetchChapterIdentity
    {
        public readonly string Url;
        public readonly string EntityUrl;
        public readonly ISingleResourceFetcher ReloopFetcher;

        public FetchChapterIdentity(string url, string entityUrl)
        {
            Url = url;
            EntityUrl = entityUrl;
            ReloopFetcher = null;
        }
        public FetchChapterIdentity(string url, string entityUrl, ISingleResourceFetcher fetcher)
        {
            Url = url;
            EntityUrl = entityUrl;
            ReloopFetcher = fetcher;
        }
    }
}
