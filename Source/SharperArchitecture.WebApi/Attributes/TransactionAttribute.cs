using System;
using System.Data;

namespace SharperArchitecture.WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute : Attribute
    {
        public TransactionAttribute(bool disabled = false)
        {
            Disabled = disabled;
        }

        public IsolationLevel? Level { get; set; }

        public bool Disabled { get; }
    }
}
