using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PowerArhitecture.Breeze.Metadata
{
    /*
    public class BaseTypeJsonConverter : MetadataJsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var result = jObject["isComplexType"] != null
                ? (StructuralType)new ComplexType()
                : new EntityType();

            FillAllProperties(result, jObject);
            return result;
        }

        public override bool CanConvert(Type objectType)
        {
            return typeof(StructuralType).IsAssignableFrom(objectType);
        }
    }*/
}
