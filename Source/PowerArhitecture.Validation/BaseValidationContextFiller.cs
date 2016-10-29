using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Validation.Specifications;

namespace PowerArhitecture.Validation
{
    public abstract class BaseValidationContextFiller<TModel> : IValidationContextFiller<TModel>
    {
        public abstract void FillContextData(TModel model, Dictionary<string, object> contextData);

        public virtual Task FillContextDataAsync(TModel model, Dictionary<string, object> contextData)
        {
            FillContextData(model, contextData);
            return Task.CompletedTask;
        }
    }
}
