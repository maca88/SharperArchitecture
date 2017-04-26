using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Breeze.Attributes
{
    public class InversePropertyName : Attribute
    {
        public InversePropertyName(string propName)
        {
            PropertyName = propName;
        }

        public string PropertyName { get; private set; }
    }
}
