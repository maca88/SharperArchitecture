using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using SharperArchitecture.Authentication.BusinessRules;
using SharperArchitecture.Authentication.Configurations;
using SharperArchitecture.Authentication.Entities;
using SharperArchitecture.Authentication.Specifications;
using SharperArchitecture.Authentication.Stores;
using SharperArchitecture.Authentication.Validators;
using SharperArchitecture.Common.Exceptions;
using SharperArchitecture.Validation.Specifications;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Extensions;
using SimpleInjector.Packaging;

namespace SharperArchitecture.Authentication
{
    public class Package : IPackage
    {
        internal static Type UserType { get; private set; }
        internal static Type RoleType { get; private set; }
        internal static Type OrganizationType { get; private set; }

        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IPasswordHasher, PasswordHasher>();
            container.RegisterSingleton<IPasswordValidator, Validators.PasswordValidator>();
            container.Register(typeof(IClaimsIdentityFactory<,>), typeof(ClaimsIdentityFactory<,>));
            container.RegisterSingleton<IAuthenticationConfiguration, AuthenticationConfiguration>();

            container.Register(typeof(IUserLoginStore<,>), typeof(DefaultUserStore<>), Lifestyle.Scoped);
            container.Register(typeof(IUserStore<,>), typeof(DefaultUserStore<>), Lifestyle.Scoped);
            container.Register(typeof(IUserClaimStore<,>), typeof(DefaultUserStore<>), Lifestyle.Scoped);
            container.Register(typeof(IUserRoleStore<,>), typeof(DefaultUserStore<>), Lifestyle.Scoped);
            container.Register(typeof(IQueryableUserStore<,>), typeof(DefaultUserStore<>), Lifestyle.Scoped);
            container.Register(typeof(IUserPasswordStore<,>), typeof(DefaultUserStore<>), Lifestyle.Scoped);
            container.Register(typeof(IUserSecurityStampStore<,>), typeof(DefaultUserStore<>), Lifestyle.Scoped);
            container.Register(typeof(UserManager<,>), typeof(UserManager<,>), Lifestyle.Scoped);

            container.Register(typeof(IRoleStore<,>), typeof(DefaultRoleStore<>), Lifestyle.Scoped);
            container.Register(typeof(IQueryableRoleStore<,>), typeof(DefaultRoleStore<>), Lifestyle.Scoped);

            container.Register(typeof(IIdentityValidator<>), typeof(IdentityValidator<>), Lifestyle.Scoped);

            // Find out the user implementation
            var userImpls = Assembly.GetExecutingAssembly()
                .GetDependentAssemblies()
                .SelectMany(o => o.GetTypes())
                .Where(o => !o.IsAbstract && !o.IsGenericType && o.IsAssignableToGenericType(typeof(User<,,,,,,>)))
                .ToList();
            if (!userImpls.Any())
            {
                throw new SharperArchitectureException(
                    $"Unable to find the the user implementation derived from {typeof(User<,,,,,,>)}");
            }
            if (userImpls.Count > 1)
            {
                throw new SharperArchitectureException(
                    $"There are more than one user implementations thats derives from {typeof(User<,,,,,,>)}. Types: {string.Join(", ", userImpls)}");
            }
            UserType = userImpls.First();
            var userGenArgs = UserType.GetGenericType(typeof(User<,,,,,,>)).GetGenericArguments();
            RoleType = userGenArgs[2];
            OrganizationType = userGenArgs[5];

            container.RegisterInitializer(service =>
            {
                service.Instance.SetMemberValue("PasswordHasher", container.GetInstance<IPasswordHasher>());
                service.Instance.SetMemberValue("PasswordValidator", container.GetInstance<IPasswordValidator>());
                service.Instance.SetMemberValue("UserValidator", container.GetInstance(typeof(IIdentityValidator<>).MakeGenericType(UserType)));
            },
                context => context.Registration.ImplementationType.IsAssignableToGenericType(typeof(UserManager<,>)));
        }
    }
}
