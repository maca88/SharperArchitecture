using System;

namespace PowerArhitecture.Domain.Specifications
{
    public interface IRevisionedEntity
    {
		int RevisionNumber { get; set; }

		DateTime RevisionDate { get; set; } 
    }
}