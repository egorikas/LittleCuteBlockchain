using Newtonsoft.Json;

namespace LittleCuteBlockchain
{
    public static class Utils
    {
        public static string ToJson<T>(this T obj)
        {
            return JsonConvert.SerializeObject(obj, Formatting.Indented);
        }
    }
}
