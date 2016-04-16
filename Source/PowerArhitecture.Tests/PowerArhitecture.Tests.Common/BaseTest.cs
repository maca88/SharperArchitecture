using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using FluentNHibernate.Automapping;
using Ninject.Modules;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Internationalization;
using PowerArhitecture.DataAccess;
using PowerArhitecture.DataAccess.Configurations;
using PowerArhitecture.DataAccess.Events;
using PowerArhitecture.DataAccess.Extensions;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Authentication.Specifications;
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
        protected static MoqMockingKernel Kernel;
        protected static IFluentDatabaseConfiguration DatabaseConfiguration;
        protected static AutomappingConfiguration AutomappingConfiguration;
        protected static List<Assembly> TestAssemblies = new List<Assembly>
        {
            typeof(I18N).Assembly
        };
        protected static List<Assembly> EntityAssemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof (Entity))
        };
        protected static List<Assembly> ConventionAssemblies = new List<Assembly>
        {
            Assembly.GetAssembly(typeof (Database)),
            Assembly.GetAssembly(typeof (Entity)),
        };

        protected static List<ISessionFactory> SessionFactories = new List<ISessionFactory>();

        static BaseTest()
        {
        }

        public static IFluentDatabaseConfiguration CreateDatabaseConfiguration(string dbName = "Test")
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

        //No need to add reference to FluentNhibernate
        protected void AddMappingStepAssembly(Assembly assembly)
        {
            AutomappingConfiguration.AddStepAssembly(assembly);
        }

        public static void BaseClassInitialize(TestContext testContext, IFluentDatabaseConfiguration configuration)
        {
            Kernel = new MoqMockingKernel();
            NHibernateProfiler.Initialize();
            AutomappingConfiguration = new AutomappingConfiguration();
            DatabaseConfiguration = configuration;
            Kernel.RegisterDatabaseConfiguration(DatabaseConfiguration.Build());
            try
            {
                Bootstrap.Bootstrapper
                    .With
                    .Ninject()
                    .WithContainer(Kernel).And
                    //.Extension(new ActionBootstrapperExtension(BeforeStartupTasks)).And
                    .StartupTasks()
                    .IncludingOnly.AssemblyRange(TestAssemblies)
                    .Start();
            }
            catch (ReflectionTypeLoadException e)
            {
                throw new Exception(string.Join(", ", e.LoaderExceptions.Select(o => o.ToString())));
            }

            SessionFactories.Add(Kernel.Get<ISessionFactory>());
        }

        public static void BaseClassCleanup()
        {
            foreach (var sessionFactory in SessionFactories)
            {
                Database.DropTables(sessionFactory);
            }
        }

        protected void HttpContextSetup()
        {
            // We need to setup the Current HTTP Context as follows:            

            // Step 1: Setup the HTTP Request
            var httpRequest = new HttpRequest("", "http://localhost/", "");

            // Step 2: Setup the HTTP Response
            var httpResponce = new HttpResponse(new StringWriter());

            // Step 3: Setup the Http Context
            var httpContext = new HttpContext(httpRequest, httpResponce);
            var sessionContainer =
                new HttpSessionStateContainer("id",
                                               new SessionStateItemCollection(),
                                               new HttpStaticObjectsCollection(),
                                               10,
                                               true,
                                               HttpCookieMode.AutoDetect,
                                               SessionStateMode.InProc,
                                               false);
            httpContext.Items["AspSession"] =
                typeof(HttpSessionState)
                .GetConstructor(
                                    BindingFlags.NonPublic | BindingFlags.Instance,
                                    null,
                                    CallingConventions.Standard,
                                    new[] { typeof(HttpSessionStateContainer) },
                                    null)
                .Invoke(new object[] { sessionContainer });

            // Step 4: Assign the Context
            HttpContext.Current = httpContext;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            foreach (var sessionFactory in SessionFactories)
            {
                Database.RecreateTables(sessionFactory);
            }
        }
        
    }
}
