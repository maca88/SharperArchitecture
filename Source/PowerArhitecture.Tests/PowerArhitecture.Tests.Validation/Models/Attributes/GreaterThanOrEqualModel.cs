using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    public class GreaterThanOrEqualModel
    {
        [GreaterThanOrEqual(10)]
        public int Value { get; set; }

        [GreaterThanOrEqual(10, IncludePropertyName = true)]
        public int Value2 { get; set; }

    }
}
