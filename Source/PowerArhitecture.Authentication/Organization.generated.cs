using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.CodeDom.Compiler;
using System.Linq.Expressions;
using System.Reflection;
using FluentNHibernate.Automapping;

namespace PowerArhitecture.Authentication.Entities
{
	[GeneratedCode("T4Template", "1.0")]
	public partial class Organization
	{

		#region OrganizationRoles

		private ISet<OrganizationRole> _organizationRoles;

        public virtual void AddOrganizationRole(OrganizationRole organizationRole)
        {
            this.AddOneToMany(o => o.OrganizationRoles, organizationRole, o => o.Organization, o=> o.RemoveOrganizationRole);
        }

        public virtual void RemoveOrganizationRole(OrganizationRole organizationRole)
        {
            this.RemoveOneToMany(o => o.OrganizationRoles, organizationRole, o => o.Organization);
        }

		#endregion

	}
}
