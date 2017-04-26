using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace SharperArchitecture.Common.Reporting
{
    public class PrintParameters<T> : Parameters<T>
    {
        public bool Preview { get; set; }
    }
}
