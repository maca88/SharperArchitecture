using System;
using PowerArhitecture.Common.Attributes;
using System.Collections.Generic;
using PowerArhitecture.Common.Specifications;
using PowerArhitecture.Domain;
using PowerArhitecture.Domain.Attributes;
using PowerArhitecture.Validation.Attributes;

namespace PowerArhitecture.Authentication.Entities
{
    [Serializable]
    public partial class Role : VersionedEntity, IRole
    {
        [Length(50)]
        [Unique]
        public virtual string Name { get; set; }

        public virtual string Description { get; set; }

        public virtual ISet<UserRole> UserRoles
        {
            get { return _userRoles ?? (_userRoles = new HashSet<UserRole>()); }
            set { _userRoles = value; }
        }

        public virtual ISet<RolePermission> RolePermissions
        {
            get { return _rolePermissions ?? (_rolePermissions = new HashSet<RolePermission>()); }
            set { _rolePermissions = value; }
        }

        public virtual ISet<PermissionPattern> PermissionPatterns
        {
            get { return _permissionPatterns ?? (_permissionPatterns = new HashSet<PermissionPattern>()); }
            set { _permissionPatterns = value; }
        } 
    }
}
