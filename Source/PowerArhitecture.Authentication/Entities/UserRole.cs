using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Entities
{
    [Ignore]
    [Serializable]
    public abstract class UserRole<TUser, TRole> : VersionedEntityWithUser<TUser>
        where TRole : IRole, IEntity, new()
        where TUser : IUser, IEntity, new()
    {
        #region User

        [ReadOnly(true)]
        public virtual long? UserId { get; set; }

        public virtual TUser User { get; set; }

        #endregion

        #region Role

        [ReadOnly(true)]
        public virtual long? RoleId { get; set; }

        public virtual TRole Role { get; set; }

        #endregion
    }
}
