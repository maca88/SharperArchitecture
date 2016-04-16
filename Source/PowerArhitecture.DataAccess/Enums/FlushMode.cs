using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.DataAccess.Enums
{
    [Serializable]
    public enum FlushMode
    {
        Unspecified = -1,
        Never = 0,
        Commit = 5,
        Auto = 10,
        Always = 20,
    }
}
