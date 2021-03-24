using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Text;

namespace Kw.Comic.Engine.Easy.Store
{
    public static class Md5Helper
    {
        private static readonly MD5 md5 = MD5.Create();

        public static string MakeMd5(string val, Encoding e)
        {
            var buffer = e.GetBytes(val);
            var bs = md5.ComputeHash(buffer);
            var sb = new StringBuilder(bs.Length * 2);
            for (int i = 0; i < bs.Length; i++)
            {
                sb.Append(bs[i].ToString("X2"));
            }
            return sb.ToString();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string MakeMd5(string val)
        {
            return MakeMd5(val, Encoding.UTF8);
        }
    }
}
