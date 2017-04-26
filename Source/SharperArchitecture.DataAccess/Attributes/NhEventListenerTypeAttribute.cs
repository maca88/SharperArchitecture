using System;

namespace SharperArchitecture.DataAccess.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class NhEventListenerTypeAttribute : Attribute
    {
        public NhEventListenerTypeAttribute(Type type)
        {
            Order = 100;
            Type = type;
        }

        public int Order { get; set; }

        public Type ReplaceListener { get; set; }

        public Type Type { get; }
    }
}
