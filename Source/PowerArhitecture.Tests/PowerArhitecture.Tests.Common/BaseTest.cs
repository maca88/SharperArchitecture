using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using PowerArhitecture.Common.Events;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.DataAccess.Settings;
using Bootstrap.Extensions.StartupTasks;
using Bootstrap.Ninject;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHibernate;
using NHibernate.Cfg;
using Ninject;
using Ninject.MockingKernel.Moq;

namespace PowerArhitecture.Tests.Common
{
    public abstract class BaseTest
    {
        protected MoqMockingKernel Kernel;
        protected ISessionFactory SessionFactory;
        protected IUnitOfWorkFactory UnitOfWorkFactory;
        protected AutomappingConfiguration AutomappingConfiguration;
        protected IPersistenceConfigurer PersistenceConfigurer;
        protected ICollection<Assembly> EntityAssemblies;
        protected ICollection<Assembly> ConventionAssemblies;

        /*DAL Settings*/
        protected Mock<HiLoIdSettings> HiLoIdSettings;
        protected Mock<ConventionsSettings> ConventionsSettings;
        protected Mock<DatabaseSettings> DatabaseSettings;

        /*Authentication Settings*/
        protected Mock<IAuthenticationSettings> AuthenticationSettings;


        protected BaseTest()
        {
            Kernel = new MoqMockingKernel();
            NHibernateProfiler.Initialize();
            EntityAssemblies = new List<Assembly>
                {
                    Assembly.GetExecutingAssembly(), 
                    Assembly.GetCallingAssembly(), 
                    Assembly.GetAssembly(typeof(Entity))
                };
            ConventionAssemblies = new List<Assembly>
                {
                    Assembly.GetAssembly(typeof (Database)), 
                    Assembly.GetAssembly(typeof (Entity)), 
                    Assembly.GetExecutingAssembly(),

                };
            AutomappingConfiguration = new AutomappingConfiguration();


            HiLoIdSettings = new Mock<HiLoIdSettings>();
            HiLoIdSettings.Setup(o => o.Enabled).Returns(true);
            HiLoIdSettings.Setup(o => o.MaxLo).Returns(100);
            HiLoIdSettings.Setup(o => o.TableName).Returns("TestHiLoIdentity");

            ConventionsSettings = new Mock<ConventionsSettings>();
            ConventionsSettings.Setup(o => o.IdDescending).Returns(true);
            ConventionsSettings.Setup(o => o.UseBuiltInPrincipal).Returns(true);
            ConventionsSettings.Setup(o => o.UniqueWithMultipleNulls).Returns(true);
            ConventionsSettings.Setup(o => o.HiLoId).Returns(HiLoIdSettings.Object);

            DatabaseSettings = new Mock<DatabaseSettings>();
            DatabaseSettings.Setup(o => o.AllowOneToOneWithoutLazyLoading).Returns(false);
            DatabaseSettings.Setup(o => o.RecreateAtStartup).Returns(true);
            DatabaseSettings.Setup(o => o.AllowOneToOneWithoutLazyLoading).Returns(false);
            DatabaseSettings.Setup(o => o.Conventions).Returns(ConventionsSettings.Object);

            AuthenticationSettings = new Mock<IAuthenticationSettings>();
            AuthenticationSettings.Setup(o => o.SystemUserPassword).Returns("Test");
            AuthenticationSettings.Setup(o => o.SystemUserName).Returns("System");

            PersistenceConfigurer = MsSqlConfiguration
                .MsSql2008
                .ConnectionString(
                    o => o
                             .Database("_PCS")
                             .Password("q1w2e3r4")
                             .Server("93.103.9.169")
                             .Username("test"));
        }

        //No need to add reference to FluentNhibernate
        protected void AddMappingStepAssembly(Assembly assembly)
        {
            AutomappingConfiguration.AddStepAssembly(assembly);
        }

        [TestInitialize]
        public void Initialize()
        {
            Bootstrap.Bootstrapper
                .With
                .Ninject()
                .WithContainer(Kernel).And
                //.AutoMapper().And
                .Extension(new ActionBootstrapperExtension(BeforeStartupTasks)).And
                .StartupTasks()
                .Start();

            UnitOfWorkFactory = Kernel.Get<IUnitOfWorkFactory>();
            AfterInitialization();
        }

        protected virtual void BeforeStartupTasks()
        {
            Kernel.Rebind<ISessionFactory>().ToMethod(context => //Simulate SessionFactoryProvider
            {
                var eventAggregator = Kernel.Get<IEventAggregator>();
                SessionFactory = Database.CreateSessionFactory(
                    new Configuration(), 
                    eventAggregator,
                    DatabaseSettings.Object,
                    EntityAssemblies,
                    ConventionAssemblies,
                    AutomappingConfiguration,
                    Directory.GetCurrentDirectory(),
                    false,
                    configuration => configuration.Database(PersistenceConfigurer));
                
                eventAggregator.SendMessage(new SessionFactoryInitializedEvent(SessionFactory));
                if (!DatabaseSettings.Object.RecreateAtStartup) return SessionFactory;
                using (var unitOfWork = UnitOfWorkFactory.GetNew())
                {
                    //SessionProvider.OnActivation(context.Kernel, session);
                    eventAggregator.SendMessage(new PopulateDbEvent(unitOfWork));
                    //SessionProvider.OnDeactivation(context.Kernel, session);
                }
                return SessionFactory;
            }).InSingletonScope();

            Kernel.Rebind<IAuthenticationSettings>().ToConstant(AuthenticationSettings.Object).InSingletonScope();
        }

        protected virtual void AfterInitialization() { }

        [TestCleanup]
        public void Cleanup()
        {
            Database.DropTables(SessionFactory);
        }
    }
}
