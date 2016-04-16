using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Domain.Enums;

namespace PowerArhitecture.Domain.Attributes
{
    public class FetchModeAttribute : Attribute
    {
        public FetchModeAttribute(FetchMode mode)
        {
            Mode = mode;
        }

        public FetchMode Mode { get; private set; }
    }
}
