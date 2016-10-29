using System;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using PowerArhitecture.Breeze;
using PowerArhitecture.Breeze.Metadata;
using Newtonsoft.Json;
using NUnit.Framework.Internal;
using NUnit.Framework;

namespace PowerArhitecture.Tests.Breeze
{
    [TestFixture]
    public class MetadataTests
    {
        [Test]
        public void Deserialize()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string schema;
            const string resourceName = "PowerArhitecture.Tests.Breeze.schema.json";
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                schema = reader.ReadToEnd();
            }
            var data = JsonConvert.DeserializeObject<MetadataSchema>(schema);
            Assert.IsNotNull(data.NamingConvention);
            Assert.IsNotNull(data.StructuralTypes);
            foreach (var type in data.StructuralTypes)
            {
                Assert.IsNotNull(type);
                foreach (var prop in type.DataProperties)
                {
                    var name = prop.Name;
                    var nameOnServer = prop.NameOnServer;
                    var complexTypeName = prop.ComplexTypeName;
                    var concurrencyMode = prop.ConcurrencyMode;
                    var dataType = prop.DataType;
                    var defaultValue = prop.DefaultValue;
                    var isNullable = prop.IsNullable;
                    var isPartOfKey = prop.IsPartOfKey;
                    var maxLength = prop.MaxLength;
                    var validators = prop.Validators;
                }
            }
            var schema2 = JsonConvert.SerializeObject(data, Formatting.Indented);
            var expected = JsonConvert.SerializeObject(JsonConvert.DeserializeObject(schema), Formatting.Indented);
            //Assert.AreEqual(expected, schema2);
        }
    }
}
