using System.Collections.Generic;
using System.Reflection;
using FluentNHibernate.Automapping;
using FluentNHibernate.Conventions;

namespace NHibernate
{
    public static class AutoPersistenceModelModelExtensions
    {
        public static AutoPersistenceModel UseOverridesFromAssemblies(this AutoPersistenceModel model, IEnumerable<Assembly> assemblies)
        {
            foreach (var assembly in assemblies)
            {
                model.UseOverridesFromAssembly(assembly);
            }
            return model;
        }

        public static AutoPersistenceModel AddConventions(this AutoPersistenceModel model, IEnumerable<IConvention> conventions)
        {
            foreach (var convention in conventions)
            {
                model.Conventions.Add(convention);
            }
            return model;
        }
    }
}
