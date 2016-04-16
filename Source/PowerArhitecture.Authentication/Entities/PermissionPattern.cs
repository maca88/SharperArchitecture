using System;
using System.ComponentModel;
using PowerArhitecture.Authentication.Specifications;
using PowerArhitecture.Common.Attributes;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Validation.Attributes;
using IRole = PowerArhitecture.Authentication.Specifications.IRole;

namespace PowerArhitecture.Authentication.Entities
{
    [Ignore]
    [Serializable]
    public abstract class PermissionPattern<TUser, TRole> : VersionedEntityWithUser<TUser>, IPermissionPattern
        where TRole : IRole, IEntity, new()
        where TUser : IUser, IEntity, new()
    {
        [NotNull]
        public virtual string Pattern { get; set; }

        #region Role

        [ReadOnly(true)]
        public virtual long RoleId { get; set; }

        [NotNull]
        public virtual TRole Role { get; set; }

        #endregion

    }
}
