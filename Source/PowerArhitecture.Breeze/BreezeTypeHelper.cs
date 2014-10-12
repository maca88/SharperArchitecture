using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Breeze.Metadata;

namespace PowerArhitecture.Breeze
{
    public class BreezeTypeHelper
    {
        private readonly static Dictionary<string, string> BreezeTypeMap = new Dictionary<string, string>() {
                    {"Byte[]", "Binary" },
                    {"BinaryBlob", "Binary" },
                    {"Timestamp", "DateTime" },
                    {"TimeAsTimeSpan", "Time" }
                };


        // Map of data type to Breeze validation type
        private readonly static Dictionary<string, string> ValidationTypeMap = new Dictionary<string, string>() {
                    {"Boolean", "bool" },
                    {"Byte", "byte" },
                    {"DateTime", "date" },
                    {"DateTimeOffset", "date" },
                    {"Decimal", "number" },
                    {"Guid", "guid" },
                    {"Int16", "int16" },
                    {"Int32", "int32" },
                    {"Int64", "integer" },
                    {"Single", "number" },
                    {"Time", "duration" },
                    {"TimeAsTimeSpan", "duration" }
                };

        public static DataType GetDataType(Type type)
        {
            string newType;
            var typeName = (BreezeTypeMap.TryGetValue(type.Name, out newType)) ? newType : type.Name;
            DataType dataType;
            return Enum.TryParse(typeName, out dataType) ? dataType : DataType.Undefined;
        }

        public static Validator GetTypeValidator(Type type)
        {
            string validationName;
            return ValidationTypeMap.TryGetValue(type.Name, out validationName) 
                ? new Validator { Name = validationName } 
                : null;

        }
    }
}
