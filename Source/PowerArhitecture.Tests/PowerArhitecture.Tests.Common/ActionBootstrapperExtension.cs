using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bootstrap.Extensions;

namespace PowerArhitecture.Tests.Common
{
    public class ActionBootstrapperExtension : IBootstrapperExtension
    {
        private readonly Action _runAction;

        public ActionBootstrapperExtension(Action runAction)
        {
            _runAction = runAction;
        }

        public void Run()
        {
            _runAction();
        }

        public void Reset()
        {
        }
    }
}
