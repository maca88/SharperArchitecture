using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExactLengthAttribute : ValidationAttribute
    {
        public ExactLengthAttribute(int length)
        {
            Length = length;
        }

        public int Length { get; private set; }
    }
}
