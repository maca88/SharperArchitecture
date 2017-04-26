using System;
using SharperArchitecture.Common.Enums;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Commands;

namespace SharperArchitecture.Common.Attributes
{
    /// <summary>
    /// Defines a life time for a type. Currently only supported for <see cref="IEventHandler{TEvent}"/>, <see cref="IAsyncEventHandler{TEvent}"></see>
    /// <see cref="ICommandHandler{TCommand}"/> and <see cref="IAsyncCommandHandler{TCommand}"/>
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
