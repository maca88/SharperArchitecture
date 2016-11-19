using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using PowerArhitecture.Breeze.Specification;
using Bootstrap.Extensions.StartupTasks;
using PowerArhitecture.Common.Attributes;

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
            var configs = _modelConfiguratorsLazy.Value;

            foreach (var modelConfigurator in configs.OrderByDescending(o =>
            {
                var attr = o.GetType().GetCustomAttribute<PriorityAttribute>();
                return attr?.Priority ?? PriorityAttribute.Default;
            }))
            {
                modelConfigurator.Configure();
            }
        }

        public void Reset()
        {
        }
    }
}
