using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.DataAccess.Configurations
{
    public static class DatabaseConfigurationKeys
    {
        public const string ValidateSchema = "PowerArhitecture.Database.ValidateSchema:bool";
        public const string RecreateSchemaAtStartup = "PowerArhitecture.Database.RecreateSchemaAtStartup:bool";
        public const string UpdateSchemaAtStartup = "PowerArhitecture.Database.UpdateSchemaAtStartup:bool";
        public const string AllowOneToOneWithoutLazyLoading = "PowerArhitecture.Database.AllowOneToOneWithoutLazyLoading:bool";
        public const string ConventionsHiLoIdEnabled = "PowerArhitecture.Database.Conventions.HiLoId.Enabled:bool";
        public const string ConventionsHiLoIdTableName = "PowerArhitecture.Database.Conventions.HiLoId.TableName:string";
        public const string ConventionsHiLoIdMaxLo = "PowerArhitecture.Database.Conventions.HiLoId.MaxLo:int";
        public const string ConventionsIdDescending = "PowerArhitecture.Database.Conventions.IdDescending:bool";
        public const string ConventionsUniqueWithMultipleNulls = "PowerArhitecture.Database.Conventions.UniqueWithMultipleNulls:bool";
        public const string ConventionsDateTimeZone = "PowerArhitecture.Database.Conventions.DateTimeZone:string";
        public const string ConventionsRequiredLastModifiedProperty = "PowerArhitecture.Database.Conventions.RequiredModifiedProperty:bool";
    }
}
