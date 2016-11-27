using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.DataAccess.Specifications;

namespace PowerArhitecture.DataAccess.Configurations
{
    public class DatabaseConfiguration
    {
        public const string DefaultName = "default";

        internal DatabaseConfiguration(Configuration configuration, string name = null)
        {
            Name = name ?? DefaultName;
            Conventions = new ConventionsConfiguration();
            AutoMappingConfiguration = new AutoMappingConfiguration();
            EntityAssemblies = new List<Assembly>();
            ConventionAssemblies = new List<Assembly>();
            NHibernateConfiguration = configuration;
            FillFromConfig();
        }

        public string Name { get; }
        public virtual List<Assembly> EntityAssemblies { get; }
        public virtual ConventionsConfiguration Conventions { get; }
        public virtual List<Assembly> ConventionAssemblies { get; }
        public virtual AutoMappingConfiguration AutoMappingConfiguration { get; }
        public virtual Configuration NHibernateConfiguration { get; }
        public virtual bool ValidateSchema { get; internal set; }
        public virtual bool RecreateAtStartup { get; internal set; }
        public virtual bool UpdateSchemaAtStartup { get; internal set; }
        public virtual bool AllowOneToOneWithoutLazyLoading { get; internal set; }
        public virtual string HbmMappingsPath { get; internal set; }
        public virtual Action<FluentConfiguration> FluentConfigurationAction { get; internal set; }
        public virtual Action<Configuration> ConfigurationCompletedAction { get; internal set; }
        public virtual Action<IFluentAutoMappingConfiguration> AutoMappingConfigurationAction { get; internal set; }
        internal virtual Dictionary<Type, Type> RepositoryTypes { get; } = new Dictionary<Type, Type>();

        private void FillFromConfig()
        {
            ValidateSchema = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.ValidateSchema);
            RecreateAtStartup = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.RecreateSchemaAtStartup);
            UpdateSchemaAtStartup = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.UpdateSchemaAtStartup);
            AllowOneToOneWithoutLazyLoading = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.AllowOneToOneWithoutLazyLoading);
            HbmMappingsPath = AppConfiguration.GetSetting<string>(DatabaseConfigurationKeys.HbmMappingsPath);
            Conventions.FillFromConfig();
        }

    }
}
