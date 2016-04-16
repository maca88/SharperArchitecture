using System;
using System.Linq;
using System.Web;
using PowerArhitecture.Authentication.Configurations;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.HttpModule;
using PowerArhitecture.Authentication.Repositories;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Authentication.Validators;
using PowerArhitecture.Common.Configuration;
using PowerArhitecture.Common.Specifications;
using Microsoft.AspNet.Identity;
using Ninject;
using Ninject.Modules;
using IUser = PowerArhitecture.Authentication.Specifications.IUser;

namespace PowerArhitecture.Authentication
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IHttpModule>().To<PrincipalHttpModule>();

            Bind<IPasswordValidator>().To<Validators.PasswordValidator>();
            Bind(typeof (IClaimsIdentityFactory<>)).To(typeof (ClaimsIdentityFactory<>));

            //if (string.IsNullOrEmpty(AppConfiguration.GetSetting<string>(AuthenticationSettingKeys.UserClass)))
            //{
            //    Bind<UserManager<User, long>>()
            //        .ToSelf()
            //        .OnActivation((context, manager) =>
            //        {
            //            manager.PasswordHasher = context.Kernel.Get<IPasswordHasher>();
            //            manager.UserValidator = context.Kernel.Get<IIdentityValidator<User>>();
            //            manager.PasswordValidator = context.Kernel.Get<IPasswordValidator>();

            //        });
            //} 

            
            Bind<IUserProvider>().To<UserProvider>().InSingletonScope();

            Bind<IUser>().ToMethod(context => context.Kernel.Get<IUserProvider>().GetCurrentUser());

            if (!Kernel.GetBindings(typeof(IAuthenticationConfiguration)).Any())
                Bind<IAuthenticationConfiguration>().To<AuthenticationConfiguration>().InSingletonScope();
        }
    }
}
