using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FootballResults.Models.Api.FootballApi.Responses.CustomConverters
{
    public class KeyValuePairConverter<TValue> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Dictionary<string, object>);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray.Load(reader);
                return new Dictionary<string, TValue>();
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return serializer.Deserialize<Dictionary<string, TValue>>(reader);
            }
            throw new JsonSerializationException("Unexpected JSON format.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dictionary = (Dictionary<string, TValue>)value;
            serializer.Serialize(writer, dictionary);
        }
    }
}
