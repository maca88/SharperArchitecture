using System;
using System.Data;

namespace SharperArchitecture.Common.Attributes
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public class TransactionAttribute : Attribute
    {
        public TransactionAttribute(bool enabled = true)
        {
            Enabled = enabled;
        }

        public IsolationLevel? Level { get; set; }

        public bool Enabled { get; }

        public bool Distributed { get; set; }


        ///// <summary>
        ///// The number of times to retry when a <see cref="NHibernate.StaleStateException" /> occurs.
        ///// </summary>
        //public int RetryTimes { get; set; }

        ///// <summary>
        ///// The delay to wait before retrying
        ///// </summary>
        //public long RetryDelay { get; set; }
    }
}
