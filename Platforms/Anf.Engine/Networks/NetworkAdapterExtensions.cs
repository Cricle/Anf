using Anf.Engine;
using System.IO;
using System.Threading.Tasks;

namespace Anf.Networks
{
    public static class NetworkAdapterExtensions
    {
        public static async Task<string> GetStringAsync(this INetworkAdapter adapter, RequestSettings settings)
        {
            using (var stream = await adapter.GetStreamAsync(settings))
            using (var sr = new StreamReader(stream))
            {
                return sr.ReadToEnd();
            }
        }
        public static async Task<T> GetObjectAsync<T>(this INetworkAdapter adapter,RequestSettings settings)
        {
            using (var stream = await adapter.GetStreamAsync(settings))
            using (var sr = new StreamReader(stream))
            {
                var str = sr.ReadToEnd();
                return JsonHelper.Deserialize<T>(str);
            }
        }
    }
}
