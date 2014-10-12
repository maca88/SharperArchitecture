using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.JsonNet;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Web.SignalR.Events;
using PowerArhitecture.Web.SignalR.StartupTasks;
using Microsoft.AspNet.Identity;
using Bootstrap.Extensions.StartupTasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Web.Infrastructure.DynamicModuleHelper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Ninject;
using Ninject.Syntax;
using Owin;

[assembly: OwinStartup(typeof(InitializationTask))]
namespace PowerArhitecture.Web.SignalR.StartupTasks
{
    public class InitializationTask : IStartupTask
    {
        private static IResolutionRoot _resolutionRoot;

        //Workaround for owin initialization w/o IoC
        private static IEventAggregator _eventAggregator;

        //This will be called by Owin
        public InitializationTask()
        {
        }

        public InitializationTask(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        //Called by Owin
        public void Configuration(IAppBuilder app) //TODO: better init
        {
            // Any connection or hub wire up and configuration should go here
            GlobalHost.DependencyResolver = _resolutionRoot.Get<IDependencyResolver>();
            // Enable the application to use a cookie to store information for the signed in user
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
            });

            var resolver = _resolutionRoot.Get<IMultipleContractResolver>(); //SignalR must have default contract resolver to work properly
            resolver.ResolveAssemblyWithResolver(ContractResolvers.DefaultResolver, typeof(Hub).Assembly);

            // Use a cookie to temporarily store information about a user logging in with a third party login provider
            //app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            app.MapSignalR(new HubConfiguration
            {
                EnableDetailedErrors = true,
                Resolver = GlobalHost.DependencyResolver
            });

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication();

            _eventAggregator.SendMessage(new OwinInitializingEvent(app));
        }

        public void Run()
        {
            _eventAggregator = _resolutionRoot.Get<IEventAggregator>();/* TODO: FIX STARTUP PROBLEM 
            var owinHttpModule =
                typeof (Microsoft.Owin.Host.SystemWeb.OwinHttpHandler).Assembly.GetType(
                    "Microsoft.Owin.Host.SystemWeb.OwinHttpModule");
            DynamicModuleUtility.RegisterModule(owinHttpModule); //Start Owin*/
        }

        public void Reset()
        {
        }
    }
}
