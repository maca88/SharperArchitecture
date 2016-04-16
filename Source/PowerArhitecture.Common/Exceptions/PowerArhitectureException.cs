using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Common.Exceptions
{
    public class PowerArhitectureException : Exception
    {
        public PowerArhitectureException(string message) : base(message)
        {
            
        }

        public PowerArhitectureException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public PowerArhitectureException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
