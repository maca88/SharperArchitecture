using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Common.AutoMapper;
using Bootstrap;
using Bootstrap.Extensions;
using Ninject.Syntax;

namespace BAF.Common.Extensions
{
    public static class BootstrapperAutoMApperExtensions
    {
        public static BootstrapperExtensions AutoMapper(this BootstrapperExtensions extensions)
        {
            return extensions.Extension(new AutoMapperExtension(Bootstrapper.RegistrationHelper));
        }
    }
}
