using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHibernate;
using Ninject;
using Ninject.MockingKernel.Moq;
using BAF.Libraries.Common.Settings;
using BAF.Libraries.DataAccess;
using BAF.Libraries.DataAccess.Configurations;
using BAF.Libraries.DataAccess.Specifications;

namespace BAF.Tests.Libraries.DataAccess
{
    public abstract class BaseTest
    {
        protected MoqMockingKernel Kernel;
        protected ISessionFactory SessionFactory;
        protected IUnitOfWorkFactory UnitOfWorkFactory;
        protected IAutomappingConfiguration AutomappingConfiguration = new AutomappingConfiguration();
        protected bool AllowOneToOneWithoutLazyLoading;
        protected DatabaseSettings DatabaseSettings;
        protected IPersistenceConfigurer PersistenceConfigurer;
        protected ICollection<Assembly> EntityAssemblies;

        protected BaseTest()
        {
            Kernel = new MoqMockingKernel();
            NHibernateProfiler.Initialize();
            EntityAssemblies = new List<Assembly> {Assembly.GetExecutingAssembly()};
            DatabaseSettings = new DatabaseSettings
                {
                    RecreateAtStartup = true,
                    AllowOneToOneWithoutLazyLoading = AllowOneToOneWithoutLazyLoading,
                    Conventions = new ConventionsSettings
                        {
                            IdDescending = true,
                            UniqueWithMultipleNulls = true,
                            HiLoId = new HiLoIdSettings
                                {
                                    Enabled = true,
                                    MaxLo = 100,
                                    TableName = "TestHiLoIdentity"
                                }
                        }
                };
            PersistenceConfigurer = MsSqlConfiguration
                .MsSql2008
                .ConnectionString(
                    o => o
                             .Database("_BAF")
                             .Password("q1w2e3r4")
                             .Server("93.103.9.169")
                             .Username("test"));
        }

        [TestInitialize]
        public void Initialize()
        {
            Kernel.Load(new NinjectRegistration());
            SessionFactory = Database.CreateSessionFactory(
                DatabaseSettings,
                EntityAssemblies,
                AutomappingConfiguration,
                Directory.GetCurrentDirectory(),
                false,
                configuration => configuration.Database(PersistenceConfigurer));
            Kernel.Rebind<ISessionFactory>().ToConstant(SessionFactory).InSingletonScope();
            UnitOfWorkFactory = Kernel.Get<IUnitOfWorkFactory>();
            AfterInitialization();
        }

        public virtual void AfterInitialization() {}

        [TestCleanup]
        public void Cleanup()
        {
            Database.DropTables(SessionFactory);
        }
    }
}
