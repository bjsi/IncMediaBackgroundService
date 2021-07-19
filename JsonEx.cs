using Newtonsoft.Json;

namespace IncMediaBackgroundService
{
    public static class JsonEx
    {
        public static string Serialize(this object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
