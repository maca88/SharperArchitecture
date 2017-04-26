using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GreaterThanAttribute : ComparisonAttribute
    {
        public GreaterThanAttribute() { }

        public GreaterThanAttribute(object value) : base(value)
        {
        }
    }
}
