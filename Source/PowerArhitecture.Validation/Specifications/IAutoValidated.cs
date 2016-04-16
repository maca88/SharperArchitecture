using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Validation.Specifications
{
    public interface IAutoValidated
    {
        bool ValidateOnUpdate { get; }

        bool ValidateOnInsert { get; }

        bool ValidateOnDelete { get; }
    }
}
