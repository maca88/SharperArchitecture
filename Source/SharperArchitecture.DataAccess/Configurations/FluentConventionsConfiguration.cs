using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.DataAccess.Specifications;

namespace SharperArchitecture.DataAccess.Configurations
{
    public class FluentConventionsConfiguration : IFluentConventionsConfiguration
    {
        private readonly ConventionsConfiguration _configuration;

        public FluentConventionsConfiguration(ConventionsConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IFluentConventionsConfiguration IdDescending(bool value = true)
        {
            _configuration.IdDescending = value;
            return this;
        }

        public IFluentConventionsConfiguration UniqueWithMultipleNulls(bool value = true)
        {
            _configuration.UniqueWithMultipleNulls = value;
            return this;
        }

        public IFluentConventionsConfiguration RequiredLastModifiedProperty(bool value = true)
        {
            _configuration.RequiredLastModifiedProperty = value;
            return this;
        }

        public IFluentConventionsConfiguration HiLoId(Action<IFluentHiLoIdConfiguration> action)
        {
            action(new FluentHiLoIdConfiguration(_configuration.HiLoId));
            return this;
        }

        public IFluentConventionsConfiguration DateTimeZone(DateTimeKind kind)
        {
            _configuration.DateTimeZone = kind.ToString();
            return this;
        }
    }
}
