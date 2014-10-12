using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Bootstrap.Extensions.Containers;

namespace BAF.Common.AutoMapper
{
    public class AutoMapperRegistration : IBootstrapperRegistration
    {
        public void Register(IBootstrapperContainerExtension containerExtension)
        {
            //containerExtension.Register<IProfileExpression>(Mapper.Configuration);
            //containerExtension.Register(Mapper.Engine);
            containerExtension.RegisterAll<Profile>();
        }
    }
}
