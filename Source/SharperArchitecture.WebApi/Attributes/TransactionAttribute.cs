using System;
using System.Data;

namespace SharperArchitecture.WebApi.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class TransactionAttribute : Attribute
    {
        public TransactionAttribute(bool enabled = true)
        {
            Enabled = enabled;
        }

        public IsolationLevel? Level { get; set; }

        public bool Enabled { get; }
    }
}
