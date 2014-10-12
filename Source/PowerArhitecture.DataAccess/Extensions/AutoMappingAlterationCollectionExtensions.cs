using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate.Automapping;

namespace FluentNHibernate.Automapping
{
    public static class AutoMappingAlterationCollectionExtensions
    {
        public static AutoMappingAlterationCollection AddFromAssemblies(this AutoMappingAlterationCollection alterationCollection, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                alterationCollection.AddFromAssembly(assembly);
            }
            return alterationCollection;
        }
    }
}
