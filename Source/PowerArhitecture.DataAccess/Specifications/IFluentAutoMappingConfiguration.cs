using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using FluentNHibernate;

namespace PowerArhitecture.DataAccess.Specifications
{
    public interface IFluentAutoMappingConfiguration
    {
        IFluentAutoMappingConfiguration ShouldMapType(Predicate<Type> predicate);
        IFluentAutoMappingConfiguration ShouldMapMember(Predicate<Member> predicate);
        IFluentAutoMappingConfiguration AddStepAssembly(Assembly assembly);
        IFluentAutoMappingConfiguration AddStepAssemblies(IEnumerable<Assembly> assemblies);
    }
}
