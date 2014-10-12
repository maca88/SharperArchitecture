using System;

namespace PowerArhitecture.DataAccess.Attributes
{
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
