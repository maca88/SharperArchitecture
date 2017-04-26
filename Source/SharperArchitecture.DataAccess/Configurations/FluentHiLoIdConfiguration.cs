using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.DataAccess.Specifications;

namespace SharperArchitecture.DataAccess.Configurations
{
    public class FluentHiLoIdConfiguration : IFluentHiLoIdConfiguration
    {
        private readonly HiLoIdConfiguration _configuration;

        public FluentHiLoIdConfiguration(HiLoIdConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IFluentHiLoIdConfiguration Enabled(bool value = true)
        {
            _configuration.Enabled = value;
            return this;
        }

        public IFluentHiLoIdConfiguration TableName(string value)
        {
            _configuration.TableName = value;
            return this;
        }

        public IFluentHiLoIdConfiguration MaxLo(int value)
        {
            _configuration.MaxLo = value;
            return this;
        }
    }
}
