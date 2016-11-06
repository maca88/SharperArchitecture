using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using Ninject.Activation;
using Ninject.Parameters;
using Ninject.Planning.Targets;
using PowerArhitecture.DataAccess.Attributes;

namespace PowerArhitecture.DataAccess.Parameters
{
    public class DatabaseConfigurationParameter : Parameter
    {
        public DatabaseConfigurationParameter(string name) : base(name, (object)null, true)
        {
        }
    }
}
