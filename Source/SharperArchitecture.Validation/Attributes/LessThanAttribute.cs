using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LessThanAttribute : ComparisonAttribute
    {
        public LessThanAttribute() { }

        public LessThanAttribute(object value) : base(value)
        {
        }
    }
}
