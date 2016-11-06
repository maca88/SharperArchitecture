using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Planning.Bindings;
using PowerArhitecture.DataAccess.Extensions;

namespace PowerArhitecture.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class DatabaseAttribute : ConstraintAttribute
    {
        public DatabaseAttribute(string configurationName)
        {
            ConfigurationName = configurationName;
        }
        public string ConfigurationName { get; }

        public override bool Matches(IBindingMetadata metadata)
        {
            return metadata.Get<string>(NinjectModuleExtensions.DatabaseConfigurationMedatadataKey, null) == ConfigurationName;
        }
    }
}
