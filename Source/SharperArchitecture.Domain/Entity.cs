using System;
using System.Xml.Serialization;
using SharperArchitecture.Domain.Attributes;

namespace SharperArchitecture.Domain
{
    [Serializable]
    [Ignore]
    public abstract class Entity : Entity<long>
    {
        public override bool IsTransient()
        {
            return Id <= 0; //Breeze will set this to a negative value
        }
    }

    [Serializable]
    [Ignore]
    public abstract class Entity<TType> : IEntity<TType>
    {
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
    }
}
