using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Common.Commands;

namespace PowerArhitecture.Tests.Common.Commands
{
    public class Model
    {
        public string Name { get; set; }
    }

    public class ModelCommand : ModelCommand<Model, bool>
    {
        public ModelCommand(Model model) : base(model)
        {
        }
    }

    public class ModelCommandHandler : BaseCommandHandler<ModelCommand, bool>
    {
        public override bool Handle(ModelCommand command)
        {
            command.Model.Name = "Test";
            return true;
        }
    }
}
