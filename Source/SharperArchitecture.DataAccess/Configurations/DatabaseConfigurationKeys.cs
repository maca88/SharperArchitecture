using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.DataAccess.Configurations
{
    public static class DatabaseConfigurationKeys
    {
        public const string ValidateSchema = "SharperArchitecture.Database.ValidateSchema:bool";
        public const string RecreateSchemaAtStartup = "SharperArchitecture.Database.RecreateSchemaAtStartup:bool";
        public const string UpdateSchemaAtStartup = "SharperArchitecture.Database.UpdateSchemaAtStartup:bool";
        public const string AllowOneToOneWithoutLazyLoading = "SharperArchitecture.Database.AllowOneToOneWithoutLazyLoading:bool";
        public const string HbmMappingsPath = "SharperArchitecture.Database.HbmMappingsPath:string";
        public const string ConventionsHiLoIdEnabled = "SharperArchitecture.Database.Conventions.HiLoId.Enabled:bool";
        public const string ConventionsHiLoIdTableName = "SharperArchitecture.Database.Conventions.HiLoId.TableName:string";
        public const string ConventionsHiLoIdMaxLo = "SharperArchitecture.Database.Conventions.HiLoId.MaxLo:int";
        public const string ConventionsIdDescending = "SharperArchitecture.Database.Conventions.IdDescending:bool";
        public const string ConventionsUniqueWithMultipleNulls = "SharperArchitecture.Database.Conventions.UniqueWithMultipleNulls:bool";
        public const string ConventionsDateTimeZone = "SharperArchitecture.Database.Conventions.DateTimeZone:string";
        public const string ConventionsRequiredLastModifiedProperty = "SharperArchitecture.Database.Conventions.RequiredLastModifiedProperty:bool";
    }
}
