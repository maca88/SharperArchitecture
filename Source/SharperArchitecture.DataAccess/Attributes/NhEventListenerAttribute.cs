using System;

namespace SharperArchitecture.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NhEventListenerAttribute : Attribute
    {
        public NhEventListenerAttribute()
        {
            Order = 100;
        }

        public int Order { get; set; }

        public Type ReplaceListener { get; set; }
    }
}
