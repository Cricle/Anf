using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Anf.Easy.Store
{
    public class MD5AddressToFileNameProvider : IAddressToFileNameProvider
    {
        public static readonly MD5AddressToFileNameProvider Instance = new MD5AddressToFileNameProvider();

        public MD5AddressToFileNameProvider()
        {
        }

        public string Convert(string address)
        {
            return Md5Helper.MakeMd5(address);
        }

        public void Dispose()
        {
        }
    }
}
