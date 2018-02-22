using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Tests.WebApi
{
    public class HttpResponseError
    {
        public string Message { get; set; }

        public string ExceptionMessage { get; set; }

        public Type ExceptionType { get; set; }

        public string StackTrace { get; set; }
    }
}
