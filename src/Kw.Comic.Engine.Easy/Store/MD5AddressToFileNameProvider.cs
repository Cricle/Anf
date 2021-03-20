using System.Security.Cryptography;
using System.Text;

namespace Kw.Comic.Engine.Easy.Store
{
    public class MD5AddressToFileNameProvider : IAddressToFileNameProvider
    {
        public static readonly MD5AddressToFileNameProvider Instance = new MD5AddressToFileNameProvider();

        private readonly MD5 mD5Crypto;

        public MD5AddressToFileNameProvider()
        {
            mD5Crypto = MD5.Create();
        }

        public string Convert(string address)
        {
            var buffer = Encoding.UTF8.GetBytes(address);
            var res=mD5Crypto.ComputeHash(buffer);
            var builder = new StringBuilder(res.Length*2);
            for (int i = 0; i < res.Length; i++)
            {
                builder.Append(res[i].ToString("X2"));
            }
            return builder.ToString();
        }

        public void Dispose()
        {
            mD5Crypto.Dispose();
        }
    }
}
