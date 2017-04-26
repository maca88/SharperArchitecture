using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class GreaterThanOrEqualAttribute : ComparisonAttribute
    {
        public GreaterThanOrEqualAttribute() { }

        public GreaterThanOrEqualAttribute(object value) : base(value)
        {
        }
    }
}
