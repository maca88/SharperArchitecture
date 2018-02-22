using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using SharperArchitecture.Common.Internationalization;
using NUnit.Framework;
using SharperArchitecture.Common;
using SharperArchitecture.Common.Configuration;
using SharperArchitecture.Common.SimpleInjector;
using SimpleInjector;
using SimpleInjector.Extensions.ExecutionContextScoping;

namespace SharperArchitecture.Tests.Common
{
    public abstract class BaseTest
    {
        protected Container Container;
        
        protected List<Assembly> TestAssemblies = new List<Assembly>
        {
            typeof(I18N).Assembly
        };
        
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Configure();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Cleanup();
        }

        protected virtual void Cleanup()
        {
            Container.Dispose();
        }

        protected virtual Container CreateContainer()
        {
            return new Container();
        }

        protected virtual void ConfigureContainer(Container container)
        {
        }

        protected virtual void Configure()
        {
            AppConfiguration.SetGetDomainAssembliesFunction(() => TestAssemblies);
            Container = CreateContainer();
            ConfigureContainer(Container);
            Container.RegisterPackages(TestAssemblies);
            Container.Verify();
        }
    }
}
