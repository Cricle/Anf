namespace Anf.ResourceFetcher.Fetchers
{
    public interface IResourceMonitorContext
    {
        string Url { get; }

        ISingleResourceFetcher ProviderFetcher { get; }

        IResourceFetchContext FetchContext { get; }
    }
}
