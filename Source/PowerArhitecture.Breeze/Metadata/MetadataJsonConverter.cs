using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerArhitecture.Breeze.Metadata
{
    public class MetadataJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var dict = value as IDictionary;
            if (dict == null) return;

            writer.WriteStartObject();

            foreach (DictionaryEntry currObj in dict)
            {
                writer.WritePropertyName(currObj.Key.ToString());
                serializer.Serialize(writer, currObj.Value);
            }

            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = (IDictionary)Activator.CreateInstance(objectType);
            FillAllProperties(type, jObject);
            return type;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType);
        }

        protected void FillAllProperties(IDictionary type, JObject jObject)
        {
            foreach (var prop in jObject.Properties())
            {
                type[prop.Name] = prop.Value;
            }
        }
    }
}
