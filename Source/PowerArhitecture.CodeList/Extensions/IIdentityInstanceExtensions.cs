using System.Collections.Generic;
using System.Reflection;
using FluentNHibernate.Conventions.Instances;
using FluentNHibernate.MappingModel;

namespace PowerArhitecture.CodeList.Extensions
{
    internal static class IIdentityInstanceExtensions
    {
         public static IEnumerable<ColumnMapping> GetColumns(this IIdentityInstance identityInstance)
         {
             return identityInstance.GetMemberValue<IEnumerable<ColumnMapping>>("mapping.Columns"); //Hack
         }
    }
}
