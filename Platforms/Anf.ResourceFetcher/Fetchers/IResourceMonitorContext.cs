namespace Anf.ResourceFetcher.Fetchers
{
    public interface IResourceMonitorContext
    {
        string Url { get; }

        IResourceFetcher ProviderFetcher { get; }

        IResourceFetchContext FetchContext { get; }
    }
}
