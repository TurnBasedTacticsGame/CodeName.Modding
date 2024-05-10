using System;
using Newtonsoft.Json;
using Object = UnityEngine.Object;

namespace CodeName.Modding.Serialization
{
    public class GameResourceConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var typedValue = (Object)value;
            if (typedValue == null)
            {
                serializer.Serialize(writer, null);

                return;
            }

            if (!GameResources.TryGetResourceKey(typedValue, out var key))
            {
                throw new GameResourceSerializationException($"Resource '{typedValue}' does not have a Resource Key. Please make sure that the Resource is part of a mod's Content folder.");
            }

            serializer.Serialize(writer, key);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var key = serializer.Deserialize<string>(reader)!;
            if (key == null)
            {
                return null;
            }

            if (!GameResources.TryGetResource(key, out var resource))
            {
                throw new GameResourceSerializationException($"Failed to load resource for Resource Key '{key}'. Please make sure that the Resource is part of a mod's Content folder.");
            }

            return resource;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(Object).IsAssignableFrom(objectType);
        }
    }
}
