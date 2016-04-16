using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using Ninject.Planning.Bindings;

namespace PowerArhitecture.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property)]
    public class NamedSessionFactoryAttribute : ConstraintAttribute
    {
        public NamedSessionFactoryAttribute(string name)
        {
            Name = name;
        }
        public string Name { get; private set; }

        public override bool Matches(IBindingMetadata metadata)
        {
            return metadata.Get<string>("SessionFactoryName", null) == Name;
        }
    }
}
