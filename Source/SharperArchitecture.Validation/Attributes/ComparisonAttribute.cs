using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Validation.Attributes
{
    public abstract class ComparisonAttribute : ValidationAttribute
    {
        protected ComparisonAttribute()
        {  
        }

        protected ComparisonAttribute(object value)
        {
            CompareToValue = value;
        }

        public object CompareToValue { get; set; }

        public string ComparsionProperty { get; set; }
    }
}
