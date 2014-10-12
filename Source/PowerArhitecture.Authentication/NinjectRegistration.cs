using System.Web;
using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Authentication.HttpModule;
using PowerArhitecture.Authentication.Managers;
using PowerArhitecture.Authentication.Repositories;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Authentication.Validators;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.DataCaching.Specifications;
using Microsoft.AspNet.Identity;
using Ninject;
using Ninject.Modules;
using IUser = PowerArhitecture.Common.Specifications.IUser;

namespace PowerArhitecture.Authentication
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IHttpModule>().To<PrincipalHttpModule>();

            Bind<IPasswordValidator>().To<Validators.PasswordValidator>();
            Bind(typeof (IClaimsIdentityFactory<>)).To(typeof (ClaimsIdentityFactory<>));

            Bind<SignInManager, SignInManager<User, long>>().To<SignInManager>();
            Bind<UserManager<User, long>>()
                .ToSelf()
                .OnActivation((context, manager) =>
                    {
                        manager.PasswordHasher = context.Kernel.Get<IPasswordHasher>();
                        manager.UserValidator = context.Kernel.Get<IIdentityValidator<User>>();
                        manager.PasswordValidator = context.Kernel.Get<IPasswordValidator>();

                    });
            Bind<IUserCache>().To<UserCache>().InSingletonScope();

            Bind<IUser>().ToMethod(context => context.Kernel.Get<IUserCache>().GetCurrentUser());
        }
    }
}
