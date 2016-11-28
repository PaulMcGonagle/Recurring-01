using Newtonsoft.Json;
using NodaTime.Serialization.JsonNet;
using NodaTime;

namespace MySerializer
{
    public static class Class1
    {
        public static string Serialize(object source)
        {
            var serializer = new JsonSerializer();
            serializer.ConfigureForNodaTime(DateTimeZoneProviders.Tzdb);

            var jsonSerializerSettings = new JsonSerializerSettings()
            {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormat = System.Runtime.Serialization.Formatters.FormatterAssemblyStyle.Simple,
            };

            return JsonConvert.SerializeObject(source, Formatting.Indented, jsonSerializerSettings);

        }

        public static T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.Objects
                });
        }
    }
}
