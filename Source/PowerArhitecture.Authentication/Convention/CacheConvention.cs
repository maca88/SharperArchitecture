using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Conventions;
using FluentNHibernate.Conventions.Instances;
using PowerArhitecture.Authentication.Configurations;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.Authentication.Convention
{
    public class CacheConvention : IClassConvention
    {
        private readonly bool _caching;

        public CacheConvention()
        {
            _caching = AppConfiguration.GetSetting<bool>(AuthenticationConfigurationKeys.Caching);
        }

        public void Apply(IClassInstance instance)
        {
            if (!_caching)
            {
                return;
            }
            var baseType = instance.EntityType?.BaseType;
            if (baseType == null || baseType.Namespace != "PowerArhitecture.Authentication.Entities")
            {
                return;
            }
            instance.Cache.ReadWrite();
        }
    }
}
