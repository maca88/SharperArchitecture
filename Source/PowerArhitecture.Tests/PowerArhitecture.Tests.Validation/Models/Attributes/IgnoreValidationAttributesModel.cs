using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Tests.Validation.Models.Attributes
{
    [IgnoreValidationAttributes(Properties = new []{"Name"})]
    public class IgnoreValidationAttributesModel
    {
        [NotNull]
        public string Name { get; set; }
    }
}
