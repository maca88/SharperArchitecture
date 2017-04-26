using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Validation.Attributes;

namespace SharperArchitecture.Tests.Validation.Models.Attributes
{
    [IgnoreValidationAttributes(Properties = new []{"Name"})]
    public class IgnoreValidationAttributesModel
    {
        [NotNull]
        public string Name { get; set; }
    }
}
