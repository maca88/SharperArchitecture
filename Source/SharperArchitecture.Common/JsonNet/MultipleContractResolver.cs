using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using SharperArchitecture.Common.Specifications;
using Newtonsoft.Json.Serialization;

namespace SharperArchitecture.Common.JsonNet
{
    public class MultipleContractResolver : IMultipleContractResolver
    {
        private readonly IDictionary<Assembly, string> _assemblies = new Dictionary<Assembly, string>();
        private readonly IDictionary<string, IContractResolver> _resolvers = new Dictionary<string, IContractResolver>();
        private string _defaultResolver; 

        public MultipleContractResolver()
        {
            _defaultResolver = ContractResolvers.DefaultResolver;
            RegisterResolvers();
        }

        private void RegisterResolvers()
        {
            _resolvers.Add(ContractResolvers.DefaultResolver, new DefaultContractResolver());
            _resolvers.Add(ContractResolvers.CamelCasePropertyNamesResolver, new CamelCasePropertyNamesContractResolver());
        }
 
        public JsonContract ResolveContract(Type type)
        {
            return _assemblies.ContainsKey(type.Assembly) 
                ? _resolvers[_assemblies[type.Assembly]].ResolveContract(type) 
                : _resolvers[_defaultResolver].ResolveContract(type);
        }

        public void RegisterResolver(string name, IContractResolver resolver)
        {
            _resolvers[name] = resolver;
        }

        public void SetDefaultResolver(string name)
        {
            if(!_resolvers.ContainsKey(name))
                throw new KeyNotFoundException(name);
            _defaultResolver = name;
        }

        public void ResolveAssemblyWithResolver(string resolverName, Assembly assembly)
        {
            if (!_resolvers.ContainsKey(resolverName))
                throw new KeyNotFoundException(resolverName);
            _assemblies.Add(assembly, resolverName);
        }
    }
}
