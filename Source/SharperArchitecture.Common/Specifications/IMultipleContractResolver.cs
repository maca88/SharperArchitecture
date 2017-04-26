using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace SharperArchitecture.Common.Specifications
{
    public interface IMultipleContractResolver : IContractResolver
    {
        void RegisterResolver(string name, IContractResolver resolver);

        void SetDefaultResolver(string name);

        void ResolveAssemblyWithResolver(string resolverName, Assembly assembly);
    }
}