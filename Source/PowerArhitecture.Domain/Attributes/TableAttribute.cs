using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Domain.Attributes
{
    public class TableAttribute : Attribute
    {
        public string Name { get; set; }

        public bool View { get; set; }
    }
}
