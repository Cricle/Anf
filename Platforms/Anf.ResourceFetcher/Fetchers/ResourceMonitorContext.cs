namespace Anf.ResourceFetcher.Fetchers
{
    internal class ResourceMonitorContext : IResourceMonitorContext
    {
        public string Url { get; set; }

        public IResourceFetcher ProviderFetcher { get; set; }

        public IResourceFetchContext FetchContext { get; set; }
    }
}
