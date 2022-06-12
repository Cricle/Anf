namespace Anf.ResourceFetcher.Fetchers
{
    public class ComicEntityUrl
    {
        public string Url { get; set; }

        public string EntityUrl { get; set; }
    }
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

        public override bool Equals(object obj)
        {
            if (obj is FetchChapterIdentity identity)
            {
                return identity.Url == Url &&
                    identity.EntityUrl == EntityUrl &&
                    ReloopFetcher == identity.ReloopFetcher;
            }
            return false;
        }
        public override int GetHashCode()
        {
            unchecked
            {
                var h = 17 * 31;
                h = h * 31 + Url?.GetHashCode() ?? 0;
                h = h * 31 + EntityUrl?.GetHashCode() ?? 0;
                h = h * 31 + ReloopFetcher?.GetHashCode() ?? 0;
                return h;
            }
        }

    }
}
