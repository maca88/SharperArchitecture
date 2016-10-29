using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PowerArhitecture.Validation.Specifications
{
    public interface IValidationContextFiller<TModel>
    {
        void FillContextData(TModel model, Dictionary<string, object> contextData);

        Task FillContextDataAsync(TModel model, Dictionary<string, object> contextData);
    }
}
