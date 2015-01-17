using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Automapping;
using PowerArhitecture.Authentication.Specifications;

namespace PowerArhitecture.Authentication.Entities
{
	[GeneratedCode("T4Template", "1.0")]
	public partial class OrganizationRole
	{

		#region Organization

        [ReadOnly(true)]
        public virtual long? OrganizationId { get; set; }

        public virtual void SetOrganization(IOrganization organization)
        {
            this.SetManyToOne(o => o.Organization, organization, o => o.RemoveOrganizationRole, o => o.OrganizationRoles);
        }

        public virtual void UnsetOrganization()
        {
            this.UnsetManyToOne(o => o.Organization, o => o.OrganizationRoles);
        }

		#endregion

		#region Role

        [ReadOnly(true)]
        public virtual long? RoleId { get; set; }

		#endregion

	}
}
