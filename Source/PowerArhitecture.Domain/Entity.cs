using System;
using System.Xml.Serialization;
using PowerArhitecture.Domain.Attributes;

namespace PowerArhitecture.Domain
{
    [Serializable]
    [Ignore]
    public abstract class Entity : Entity<long>
    {
    }

    /// <summary>
    /// Base class for entities
    /// For a discussion of this object, see 
    /// http://devlicio.us/blogs/billy_mccafferty/archive/2007/04/25/using-equals-gethashcode-effectively.aspx
    /// </summary>
    [Serializable]
    [Ignore]
    public abstract class Entity<TType> : IEntity<TType>
    {
        /// <summary>
        ///     To help ensure hashcode uniqueness, a carefully selected random number multiplie r
        ///     is used within the calculation.  Goodrich and Tamassia's Data Structures and
        ///     Algorithms in Java asserts that 31, 33, 37, 39 and 41 will produce the fewest number
        ///     of collissions.  See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        ///     for more information.
        /// </summary>
        private const int HashMultiplier = 31;

        private int? _cachedHashcode;

        /// <summary>
        ///     Setter is protected to allow unit tests to set this property via reflection and to allow 
        ///     domain objects more flexibility in setting this for those objects with assigned Ids.
        ///     It's virtual to allow NHibernate-backed objects to be lazily loaded.
        /// 
        ///     This is ignored for XML serialization because it does not have a public setter (which is very much by design).
        ///     See the FAQ within the documentation if you'd like to have the Id XML serialized.
        /// </summary>
        [XmlIgnore]
        public virtual TType Id { get; protected set; }


        /// <summary>
        /// Check if this entity is transient, ie, without identity at this moment
        /// </summary>
        /// <returns>True if entity is transient, else false</returns>
        public virtual bool IsTransient()
        {
            return Id == null || Id.Equals(default(TType));
        }

        public virtual object GetId()
        {
            return Id;
        }

        public virtual Type GetIdType()
        {
            return typeof(TType);
        }
        
        public override bool Equals(object obj)
        {
            var compareTo = obj as IEntity;
            if (ReferenceEquals(this, compareTo))
                return true;
            if (compareTo == null || !(GetType() == compareTo.GetTypeUnproxied()))
                return false;
            return HasSameNonDefaultIdAs(compareTo);
        }

        public override int GetHashCode()
        {
            if (_cachedHashcode.HasValue)
                return _cachedHashcode.Value;
            if (IsTransient())
                _cachedHashcode = base.GetHashCode();
            else
            {
                unchecked
                {
                    // It's possible for two objects to return the same hash code based on 
                    // identically valued properties, even if they're of two different types, 
                    // so we include the object's type in the hash calculation
                    var hashCode = GetType().GetHashCode();
                    _cachedHashcode = (hashCode * HashMultiplier) ^ Id.GetHashCode();
                }
            }
            return _cachedHashcode.Value;
        }

        /// <summary>
        ///     Returns true if self and the provided entity have the same Id values 
        ///     and the Ids are not of the default Id value
        /// </summary>
        private bool HasSameNonDefaultIdAs(IEntity compareTo)
        {
            return !IsTransient() && !compareTo.IsTransient() && Id.Equals(compareTo.GetId());
        }

        /// <summary>
        ///     When NHibernate proxies objects, it masks the type of the actual entity object.
        ///     This wrapper burrows into the proxied object to get its actual type.
        /// 
        ///     Although this assumes NHibernate is being used, it doesn't require any NHibernate
        ///     related dependencies and has no bad side effects if NHibernate isn't being used.
        /// 
        ///     Related discussion is at http://groups.google.com/group/sharp-architecture/browse_thread/thread/ddd05f9baede023a ...thanks Jay Oliver!
        /// </summary>
        public virtual Type GetTypeUnproxied()
        {
            return GetType();
        }
    }
}
