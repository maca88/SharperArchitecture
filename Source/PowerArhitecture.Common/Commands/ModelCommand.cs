using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Commands
{
    public class ModelCommand<TModel> : ICommand
    {
        public ModelCommand(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; }
    }

    public class ModelCommand<TModel, TResult> : ICommand<TResult>
    {
        public ModelCommand(TModel model)
        {
            Model = model;
        }

        public TModel Model { get; }
    }
}
