using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FootballResults.Models.Api.FootballApi.Responses.CustomConverters
{
    public class ResponseConverter<TResponseItem> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TResponseItem);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray.Load(reader);
                return new List<TResponseItem>();
            }
            else if (reader.TokenType == JsonToken.StartObject)
            {
                return serializer.Deserialize<TResponseItem>(reader);
            }
            throw new JsonSerializationException("Unexpected JSON format.");
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var innerResponse = (TResponseItem)value;
            serializer.Serialize(writer, value);
        }
    }
}
