using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Formatting;
using System.Reflection;
using System.Web;
using System.Web.Http;
using FluentNHibernate.Cfg.Db;
using HibernatingRhinos.Profiler.Appender.NHibernate;
using Microsoft.Owin;
using NHibernate.Cfg;
using Owin;
using SharperArchitecture.DataAccess;
using SharperArchitecture.DataAccess.Configurations;
using SharperArchitecture.DataAccess.Extensions;
using SharperArchitecture.Domain;
using SharperArchitecture.Tests.WebApi.Server;
using SimpleInjector;
using SimpleInjector.Integration.WebApi;

[assembly: OwinStartup(typeof(SingleDatabaseStartup))]
namespace SharperArchitecture.Tests.WebApi.Server
{
    public class SingleDatabaseStartup
    {
        public static Container Container { get; private set; }

        private List<Assembly> ConventionAssemblies = new List<Assembly>
        {
            Assembly.GetAssembly(typeof (Database)),
            Assembly.GetAssembly(typeof (Entity))
        };

        private List<Assembly> EntityAssemblies = new List<Assembly>
        {
            Assembly.GetExecutingAssembly(),
            Assembly.GetAssembly(typeof (Entity))
        };

        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "{controller}/{action}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
            NHibernateProfiler.Initialize();

            Container = new Container();
            Container.Options.DefaultScopedLifestyle = new WebApiRequestLifestyle();

            Container.RegisterDatabaseConfiguration(GetDatabaseConfiguration());
            Container.RegisterSingleton(config);
            Container.RegisterHttpConfiguration(config);
            Container.RegisterPackages();

            Container.Verify();

            config.DependencyResolver = new SimpleInjectorWebApiDependencyResolver(Container);

            config.Formatters.Clear();
            config.Formatters.Add(new JsonMediaTypeFormatter());

            appBuilder.UseWebApi(config);
        }

        private DatabaseConfiguration GetDatabaseConfiguration(string dbName = "bar")
        {
            return FluentDatabaseConfiguration.Create(new Configuration()
                    .SetProperty(NHibernate.Cfg.Environment.GenerateStatistics, "true"))
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
                .HbmMappingsPath(".\\Mappings")
                .Conventions(c => c
                    .IdDescending()
                    .UniqueWithMultipleNulls()
                    .HiLoId(h => h
                        .Enabled()
                        .MaxLo(100)
                        .TableName("TestHiLoIdentity")
                    )
                )
                .Build();
        }
    }
}