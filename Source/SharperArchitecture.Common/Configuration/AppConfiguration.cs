using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using log4net;

namespace SharperArchitecture.Common.Configuration
{
    public class AppConfiguration
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AppConfiguration));
        private static Func<IEnumerable<Assembly>> _domainAssembliesFunc;

        //without a static constructor static fields will not be initialized
        static AppConfiguration()
        {
            _domainAssembliesFunc = AppDomain.CurrentDomain.GetAssemblies;
        }

        public static void SetGetDomainAssembliesFunction(Func<IEnumerable<Assembly>> func)
        {
            if (func == null)
            {
                return;
            }
            _domainAssembliesFunc = func;
        }

        public static IEnumerable<Assembly> GetDomainAssemblies()
        {
            return _domainAssembliesFunc().Where(o => !o.IsDynamic);
        } 

        public static Dictionary<string, object> GetSettings(string prefix = "")
        {
            var result = new Dictionary<string, object>();
            foreach (string key in ConfigurationManager.AppSettings)
            {
                if(!string.IsNullOrEmpty(prefix) && !key.StartsWith(prefix)) continue;
                var fullnameTypePair = key.Split(':');
                var toSkip = prefix?.Split('.').Length ?? 0;
                var name = string.Join(".", fullnameTypePair[0].Split('.').Skip(toSkip));
                var type = fullnameTypePair[1];
                var value = typeof (AppConfiguration)
                    .GetMethod("GetSetting")
                    .MakeGenericMethod(type.GetTypeFromSimpleName())
                    .Invoke(null, new object[] {key});
                result.Add(name, value);
            }
            return result;
        }
        public static T GetSetting<T>(string name)
        {
            try
            {
                return ConfigurationManager.AppSettings[name].ConvertTo<T>();
            }
            catch (Exception e)
            {
                Logger.Error($"Error occurred while getting setting with name '{name}'. Details: {e}");
                return default(T);
            }
        }
    }
}
