using Newtonsoft.Json;

namespace IncMediaBackgroundService.Helpers
{
    public static class JsonEx
    {
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(this string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}
