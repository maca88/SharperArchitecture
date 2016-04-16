using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using PowerArhitecture.DataAccess.Configurations;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IFluentDatabaseConfiguration
    {
        IFluentDatabaseConfiguration ValidateSchema(bool value = true);
        IFluentDatabaseConfiguration RecreateAtStartup(bool value = true);
        IFluentDatabaseConfiguration UpdateSchemaAtStartup(bool value = true);
        IFluentDatabaseConfiguration AllowOneToOneWithoutLazyLoading(bool value = true);
        IFluentDatabaseConfiguration AutomappingConfiguration(IAutomappingConfiguration value);
        IFluentDatabaseConfiguration AddEntityAssembly(Assembly assembly);
        IFluentDatabaseConfiguration AddEntityAssemblies(IEnumerable<Assembly> assemblies);
        IFluentDatabaseConfiguration AddConventionAssembly(Assembly assembly);
        IFluentDatabaseConfiguration AddConventionAssemblies(IEnumerable<Assembly> assemblies);
        IFluentDatabaseConfiguration HbmMappingsPath(string path);
        IFluentDatabaseConfiguration Conventions(Action<IFluentConventionsConfiguration> action);
        IFluentDatabaseConfiguration FluentNHibernate(Action<FluentConfiguration> action);
        DatabaseConfiguration Build();
    }
}
