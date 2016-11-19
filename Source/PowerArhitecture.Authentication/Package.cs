using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using PowerArhitecture.Authentication.Configurations;
using PowerArhitecture.Authentication.Specifications;
using SimpleInjector;
using SimpleInjector.Packaging;

namespace PowerArhitecture.Authentication
{
    public class Package : IPackage
    {
        public void RegisterServices(Container container)
        {
            container.RegisterSingleton<IPasswordHasher, PasswordHasher>();
            container.RegisterSingleton<IPasswordValidator, Validators.PasswordValidator>();
            container.Register(typeof(IClaimsIdentityFactory<>), typeof(ClaimsIdentityFactory<>));
            container.Register<IAuthenticationConfiguration, AuthenticationConfiguration>();
        }
    }
}
