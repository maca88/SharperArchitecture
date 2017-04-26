using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharperArchitecture.Common.Exceptions
{
    public class SharperArchitectureException : Exception
    {
        public SharperArchitectureException(string message) : base(message)
        {
            
        }

        public SharperArchitectureException(string message, params object[] args) : base(string.Format(message, args))
        {
        }

        public SharperArchitectureException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
