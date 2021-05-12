namespace Anf.ResourceFetcher.Fetchers
{
    public interface IValueResourceMonitorContext<TValue> : IResourceMonitorContext
    {
        TValue Value { get; }
    }
}
