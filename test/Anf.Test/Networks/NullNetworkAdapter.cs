using Anf.Networks;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Anf.Test.Networks
{
    internal class NullNetworkAdapter : INetworkAdapter
    {
        public Task<Stream> GetStreamAsync(RequestSettings settings)
        {
            var mem = new MemoryStream();
            var buffer = Encoding.UTF8.GetBytes(settings.Address);
            mem.Write(buffer,0, buffer.Length);
            mem.Seek(0, SeekOrigin.Begin);
            return Task.FromResult<Stream>(mem);
        }
    }
}
