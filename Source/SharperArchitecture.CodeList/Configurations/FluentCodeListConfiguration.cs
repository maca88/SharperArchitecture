using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.CodeList.Specifications;

namespace SharperArchitecture.CodeList.Configurations
{
    public class FluentCodeListConfiguration : IFluentCodeListConfiguration
    {
        private readonly CodeListConfiguration _configuration = new CodeListConfiguration();

        public static IFluentCodeListConfiguration Create()
        {
            return new FluentCodeListConfiguration();
        }

        //public IFluentCodeListConfiguration EnableCache(bool value = true)
        //{
        //    _configuration.EnableCache = value;
        //    return this;
        //}

        public CodeListConfiguration Build()
        {
            return _configuration;
        }
    }
}
