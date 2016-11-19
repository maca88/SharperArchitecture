using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class NamedAttribute : Attribute
    {
        public NamedAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; }
    }
}
