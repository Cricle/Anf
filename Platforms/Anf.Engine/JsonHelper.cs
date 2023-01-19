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