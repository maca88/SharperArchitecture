using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Validation.Attributes;

namespace SharperArchitecture.Tests.Validation.Models.Attributes
{
    public class RegularExpressionModel
    {
        [RegularExpression("[d]+")]
        public string Name { get; set; }

        [RegularExpression("[d]+", IncludePropertyName = true)]
        public string Name2 { get; set; }
    }
}
