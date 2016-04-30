using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider.NH;
using PowerArhitecture.Breeze.Specification;

namespace PowerArhitecture.Breeze
{
    public abstract class BreezeModelConfiguratorBase<TModel> : IBreezeModelConfigurator
    {
        protected BreezeModelConfiguratorBase()
        {
            Configuration = BreezeModelConfigurator.Configure<TModel>();
        }

        protected IModelConfiguration<TModel> Configuration { get; private set; }

        public abstract void Configure();
    }
}
