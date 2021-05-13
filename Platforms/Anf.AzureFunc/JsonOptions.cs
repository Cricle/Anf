using System.Text.Json;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using System.Text;

namespace Anf.AzureFunc
{
    internal static class JsonOptions
    {
        public static readonly Encoding Gb231 = Encoding.GetEncoding("gb2312");
        public static readonly JsonSerializerOptions DefaultOption = new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
        };
    }
}
