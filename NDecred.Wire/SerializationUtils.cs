using System.IO;
using Newtonsoft.Json;

namespace NDecred.Wire
{
    public static class SerializationUtils
    {        
        public static string Serialize<T>(this T obj) where T : NetworkEncodable
        {
            return JsonConvert.SerializeObject(obj);
        }

        public static T Deserialize<T>(string val) where T : NetworkEncodable
        {
            return JsonConvert.DeserializeObject<T>(val);
        }

        public static T Clone<T>(T obj) where T : NetworkEncodable
        {
            return Deserialize<T>(Serialize(obj));
        }
    }
}