using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using PowerArhitecture.Common.Internationalization;
using Bootstrap.Extensions.StartupTasks;
using Bootstrap.Ninject;
using Ninject;
using Ninject.MockingKernel.Moq;
using NUnit.Framework;
using PowerArhitecture.Common;

namespace PowerArhitecture.Tests.Common
{
    public abstract class BaseTest
    {
        protected MoqMockingKernel Kernel;
        
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
            foreach (var module in Kernel.GetModules())
            {
                Kernel.Unload(module.Name);
            }
            Kernel.Dispose(true);
            Bootstrap.Bootstrapper.Reset();
        }

        protected virtual void Configure()
        {
            Kernel = Kernel ?? new MoqMockingKernel();
            ConfigureKernel(Kernel);
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
            catch (Exception xe)
            {

            }
        }

        protected virtual void ConfigureKernel(MoqMockingKernel kernel)
        {
            
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

    }
}
