using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class IsolationLevelAttribute : Attribute
    {
        public IsolationLevelAttribute(IsolationLevel isolationLevel)
        {
            Level = isolationLevel;
        }

        public IsolationLevel Level { get; }
    }
}
