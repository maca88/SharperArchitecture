using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using NHibernate.Cfg;
using PowerArhitecture.Common.Configuration;

namespace PowerArhitecture.DataAccess.Configurations
{
    public class DatabaseConfiguration
    {
        public DatabaseConfiguration(Configuration configuration)
        {
            Conventions = new ConventionsConfiguration();
            EntityAssemblies = new List<Assembly>();
            ConventionAssemblies = new List<Assembly>();
            NHibernateConfiguration = configuration;
            FillFromConfig();
        }

        public virtual bool ValidateSchema { get; set; }
        public virtual bool RecreateAtStartup { get; set; }
        public virtual bool UpdateSchemaAtStartup { get; set; }
        public virtual bool AllowOneToOneWithoutLazyLoading { get; set; }
        public virtual List<Assembly> EntityAssemblies { get; private set; }
        public virtual ConventionsConfiguration Conventions { get; private set; }
        public virtual List<Assembly> ConventionAssemblies { get; private set; }
        public virtual IAutomappingConfiguration AutomappingConfiguration { get; set; }
        public virtual string HbmMappingsPath { get; set; }
        public virtual Action<FluentConfiguration> FluentConfigurationAction { get; set; }
        public virtual Configuration NHibernateConfiguration { get; private set; }
        public virtual Action<Configuration> ConfigurationCompletedAction { get; set; }

        private void FillFromConfig()
        {
            ValidateSchema = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.ValidateSchema);
            RecreateAtStartup = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.RecreateSchemaAtStartup);
            UpdateSchemaAtStartup = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.UpdateSchemaAtStartup);
            AllowOneToOneWithoutLazyLoading = AppConfiguration.GetSetting<bool>(DatabaseConfigurationKeys.AllowOneToOneWithoutLazyLoading);
            Conventions.FillFromConfig();
        }

    }
}
