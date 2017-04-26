using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Configuration;

namespace System.Reflection
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Assembly> GetDependentAssemblies(this Assembly analyzedAssembly)
        {
            return AppConfiguration.GetDomainAssemblies()
                .Where(a => GetNamesOfAssembliesReferencedBy(a)
                                    .Contains(analyzedAssembly.FullName));
        }

        private static IEnumerable<string> GetNamesOfAssembliesReferencedBy(Assembly assembly)
        {
            return assembly.GetReferencedAssemblies()
                .Select(assemblyName => assemblyName.FullName);
        }
    }
}
