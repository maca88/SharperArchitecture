using System;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Domain.Specifications
{
    public interface IRevisionEntity
    {
        long RevisionTimestamp { get; }
    }
}