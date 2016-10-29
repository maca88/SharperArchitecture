using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess
{
    public class SessionContext
    {
        public IResolutionRoot ResolutionRoot { get; set; }

        public CultureInfo CurrentCultureInfo { get; set; }

        public bool IsManaged { get; set; }
    }
}
