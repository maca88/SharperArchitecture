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
    public class NamedSessionFactoryParameter : Parameter//, IConstructorArgument
    {
        public NamedSessionFactoryParameter(string name) : base(name, (object)null, true)
        {
        }

        //public bool AppliesToTarget(IContext context, ITarget target)
        //{
        //    //Apply only when there is a ISession parameter without a NamedSessionFactoryAttribute attribute
        //    if (!typeof(ISession).IsAssignableFrom(target.Type) || target.GetCustomAttributes(typeof(NamedSessionFactoryAttribute), true).Any()) 
        //        return false;
        //    return true;
        //}
    }
}
