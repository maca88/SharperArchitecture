using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using NHibernate;
using NHibernate.Cfg;
using Ninject;
using Ninject.MockingKernel.Moq;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;

namespace PowerArhitecture.Tests.Common
{
    public abstract class DatabaseBaseTest : BaseTest
    {
        protected IFluentDatabaseConfiguration DatabaseConfiguration;
        protected AutomappingConfiguration AutomappingConfiguration = new AutomappingConfiguration();
        protected List<Assembly> EntityAssemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof (Entity))
        };
        protected List<Assembly> ConventionAssemblies = new List<Assembly>
        {
            Assembly.GetAssembly(typeof (Database)),
            Assembly.GetAssembly(typeof (Entity))
        };
        protected List<ISessionFactory> SessionFactories = new List<ISessionFactory>();

        protected virtual IFluentDatabaseConfiguration CreateDatabaseConfiguration(string dbName = "Test")
        {
            return FluentDatabaseConfiguration.Create(new Configuration())
                .AddConventionAssemblies(ConventionAssemblies)
                .AddEntityAssemblies(EntityAssemblies)
                .FluentNHibernate(f => f
                    .Database(MsSqlConfiguration
                        .MsSql2008
                        //.ConnectionString(string.Format("Data Source=(local);Initial Catalog={0};Integrated Security=true", dbName))
                        .ConnectionString(o => o
                            .Database(dbName)
                            .Server("(local)")
                            .TrustedConnection()
                        )
                    )
                )
                .RecreateAtStartup()
                .AutomappingConfiguration(AutomappingConfiguration)
                .HbmMappingsPath(".\\Mappings")
                .Conventions(c => c
                    .IdDescending()
                    .UniqueWithMultipleNulls()
                    .HiLoId(h => h
                        .Enabled()
                        .MaxLo(100)
                        .TableName("TestHiLoIdentity")
                    )
                );
        }

        protected virtual IFluentDatabaseConfiguration GetDatabaseConfiguration()
        {
            return CreateDatabaseConfiguration();
        }

        //No need to add reference to FluentNhibernate
        protected void AddMappingStepAssembly(Assembly assembly)
        {
            AutomappingConfiguration.AddStepAssembly(assembly);
        }

        protected override void Configure()
        {
            Kernel = new MoqMockingKernel();
            NHibernateProfiler.Initialize();
            DatabaseConfiguration = GetDatabaseConfiguration();
            Kernel.RegisterDatabaseConfiguration(DatabaseConfiguration.Build());

            base.Configure();

            if (DatabaseConfiguration != null)
            {
                var sessionFactory = Kernel.Get<ISessionFactory>();
                SessionFactories.Add(sessionFactory);
                FillData(sessionFactory);
            }
        }

        protected virtual void FillData(ISessionFactory sessionFactory)
        {
            
        }

        protected override void Cleanup()
        {
            /*
            foreach (var sessionFactory in SessionFactories)
            {
                Database.RecreateTables(sessionFactory);
            }*/
            foreach (var sessionFactory in SessionFactories)
            {
                Database.DropTables(sessionFactory);
                Database.RemoveSessionFactory(sessionFactory);
            }
            
            base.Cleanup();
        }
    }
}
