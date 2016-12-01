using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.WebApi.Specifications
{
    public interface IRequestMessageProvider
    {
        HttpRequestMessage CurrentMessage { get; }
    }
}
