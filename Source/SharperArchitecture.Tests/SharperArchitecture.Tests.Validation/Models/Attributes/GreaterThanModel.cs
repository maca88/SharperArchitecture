using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Validation.Attributes;

namespace SharperArchitecture.Tests.Validation.Models.Attributes
{
    public class GreaterThanModel
    {
        [GreaterThan(10)]
        public int Value { get; set; }

        [GreaterThan(10, IncludePropertyName = true)]
        public int Value2 { get; set; }

    }
}
