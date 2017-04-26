using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using SharperArchitecture.DataAccess.Configurations;

namespace SharperArchitecture.DataAccess.Specifications
{
    public interface IFluentDatabaseConfiguration
    {
        string Name { get; }
        IFluentDatabaseConfiguration ValidateSchema(bool value = true);
        IFluentDatabaseConfiguration RecreateAtStartup(bool value = true);
        IFluentDatabaseConfiguration UpdateSchemaAtStartup(bool value = true);
        IFluentDatabaseConfiguration AllowOneToOneWithoutLazyLoading(bool value = true);
        IFluentDatabaseConfiguration AutomappingConfiguration(Action<IFluentAutoMappingConfiguration> action);
        IFluentDatabaseConfiguration AddEntityAssembly(Assembly assembly);
        IFluentDatabaseConfiguration AddEntityAssemblies(IEnumerable<Assembly> assemblies);
        IFluentDatabaseConfiguration AddConventionAssembly(Assembly assembly);
        IFluentDatabaseConfiguration AddConventionAssemblies(IEnumerable<Assembly> assemblies);
        IFluentDatabaseConfiguration HbmMappingsPath(string path);
        IFluentDatabaseConfiguration Conventions(Action<IFluentConventionsConfiguration> action);
        IFluentDatabaseConfiguration FluentNHibernate(Action<FluentConfiguration> action);
        IFluentDatabaseConfiguration ConfigurationCompletedAction(Action<Configuration> action);
        DatabaseConfiguration Build();
    }
}
