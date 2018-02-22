using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HiLoAttribute : Attribute
    {
        public HiLoAttribute()
        {
        }

        public HiLoAttribute(int maxLo)
        {
            MaxLo = maxLo;
        }

        public int MaxLo { get; }

        public bool Disabled { get; set; }
    }
}
