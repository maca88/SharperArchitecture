using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.WebApi.Specifications;
using SimpleInjector;

namespace PowerArhitecture.WebApi.Internal
{
    internal sealed class RequestMessageProvider : IRequestMessageProvider
    {
        private readonly Container _container;

        public RequestMessageProvider(Container container)
        {
            _container = container;
        }

        public HttpRequestMessage CurrentMessage => _container.GetCurrentHttpRequestMessage();
    }
}
