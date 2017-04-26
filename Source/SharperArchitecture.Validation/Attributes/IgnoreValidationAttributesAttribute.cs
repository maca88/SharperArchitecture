using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Validation.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IgnoreValidationAttributesAttribute : Attribute
    {
        public IgnoreValidationAttributesAttribute()
        {
        }

        public string[] Properties { get; set; } 
    }
}
