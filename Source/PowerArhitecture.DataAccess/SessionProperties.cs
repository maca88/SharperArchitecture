using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess
{
    public class SessionProperties
    {
        public IResolutionRoot SessionResolutionRoot { get; set; }

        public bool IsManaged { get; set; }
    }
}
