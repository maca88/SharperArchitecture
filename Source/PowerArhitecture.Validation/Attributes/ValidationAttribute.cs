using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Validation.Attributes
{
    public abstract class ValidationAttribute : Attribute
    {
        /// <summary>
        /// Whether include property name in error message
        /// </summary>
        public bool IncludePropertyName { get; set; }
    }
}
