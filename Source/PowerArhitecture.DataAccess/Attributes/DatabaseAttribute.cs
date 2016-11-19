using System;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.DataAccess.Extensions;

namespace PowerArhitecture.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class DatabaseAttribute : NamedAttribute
    {
        public DatabaseAttribute(string configurationName) : base(configurationName)
        {
        }
    }
}
