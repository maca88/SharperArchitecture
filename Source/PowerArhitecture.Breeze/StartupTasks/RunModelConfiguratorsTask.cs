using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PowerArhitecture.Breeze.Specification;
using Bootstrap.Extensions.StartupTasks;

namespace PowerArhitecture.Breeze.StartupTasks
{
    public class RunModelConfiguratorsTask : IStartupTask
    {
        private readonly Lazy<IEnumerable<IBreezeModelConfigurator>> _modelConfiguratorsLazy;

        public RunModelConfiguratorsTask(Lazy<IEnumerable<IBreezeModelConfigurator>> modelConfiguratorsLazy)
        {
            _modelConfiguratorsLazy = modelConfiguratorsLazy;
        }

        public void Run()
        {
            foreach (var modelConfigurator in _modelConfiguratorsLazy.Value)
            {
                modelConfigurator.Configure();
            }
        }

        public void Reset()
        {
        }
    }
}
