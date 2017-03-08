using System;
using PowerArhitecture.Common.Enums;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Attributes
{
    /// <summary>
    /// Defines a life time for a type. Currently only supported for <see cref="IEventHandler{TEvent}"/> and <see cref="IAsyncEventHandler{TEvent}"/>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class LifetimeAttribute : Attribute
    {
        public LifetimeAttribute(Lifetime lifetime)
        {
            Lifetime = lifetime;
        }

        public Lifetime Lifetime { get; }
    }
}
