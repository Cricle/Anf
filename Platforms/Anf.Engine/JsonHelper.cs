#if StandardLib
using System.Text.Json;
namespace Anf.Engine
{
    public static class JsonHelper
    {
        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value);
        }
        public static T Deserialize<T>(string str)
        {
            return JsonSerializer.Deserialize<T>(str);
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
