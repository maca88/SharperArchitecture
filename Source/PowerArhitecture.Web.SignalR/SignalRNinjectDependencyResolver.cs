using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Ninject;
using Ninject.Syntax;

namespace PowerArhitecture.Web.SignalR
{
    public class SignalRNinjectDependencyResolver : DefaultDependencyResolver
    {
        private readonly IResolutionRoot _resolutionRoot;

        public SignalRNinjectDependencyResolver(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public override object GetService(Type serviceType)
        {
            return _resolutionRoot.TryGet(serviceType) ?? base.GetService(serviceType);
        }

        public override IEnumerable<object> GetServices(Type serviceType)
        {
            return _resolutionRoot.GetAll(serviceType).Concat(base.GetServices(serviceType));
        }
        /*
        public override void Register(Type serviceType, Func<object> activator)
        {
            if (!_kernel.GetBindings(serviceType).Any())
                _kernel.Bind(serviceType).ToMethod(context => activator);
            else
                return;
        }

        public override void Register(Type serviceType, IEnumerable<Func<object>> activators)
        {
            if (!_kernel.GetBindings(serviceType).Any())
                base.Register(serviceType, activators);
            else
                return;
        }*/
    }
}
