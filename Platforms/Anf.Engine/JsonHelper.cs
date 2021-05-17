#if StandardLib
using System.Text.Json;
namespace Anf.Engine
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T value, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Serialize(value, options);
        }
        public static T Deserialize<T>(string str, JsonSerializerOptions options = null)
        {
            return JsonSerializer.Deserialize<T>(str, options);
        }
    }
}
#else
using Newtonsoft.Json;
namespace Anf.Engine
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T value)
        {
            return JsonConvert.SerializeObject(value);
        }
        public static T Deserialize<T>(string str)
        {
            return JsonConvert.DeserializeObject<T>(str);
        }
    }
}
#endif
