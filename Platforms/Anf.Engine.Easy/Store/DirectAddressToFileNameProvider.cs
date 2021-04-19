namespace Anf.Easy.Store
{
    public class DirectAddressToFileNameProvider : IAddressToFileNameProvider
    {
        public static readonly DirectAddressToFileNameProvider Instance = new DirectAddressToFileNameProvider();

        public string Convert(string address)
        {
            return PathHelper.EnsureName(address);
        }

        public void Dispose()
        {
        }
    }
}
