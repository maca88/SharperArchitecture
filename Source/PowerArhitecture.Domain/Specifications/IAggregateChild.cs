using System;
using System.Linq.Expressions;
using NHibernate.Criterion;

namespace PowerArhitecture.Domain.Specifications
{
    public interface IAggregateChild
    {
        object AggregateRoot { get; }
    }
}