using System;
using System.Linq;
using System.Web;
using PowerArhitecture.Authentication.Configurations;
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

            if (!Kernel.GetBindings(typeof(IAuthenticationConfiguration)).Any())
                Bind<IAuthenticationConfiguration, AuthenticationConfiguration>().To<AuthenticationConfiguration>().InSingletonScope();
        }
    }
}
