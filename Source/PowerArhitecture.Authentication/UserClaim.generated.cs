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
	public partial class UserClaim
	{

		#region User

        [ReadOnly(true)]
        public virtual long? UserId { get; set; }

		#endregion

	}
}
