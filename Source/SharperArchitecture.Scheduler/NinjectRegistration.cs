using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.Scheduler.Factories;
using BAF.Scheduler.Providers;
using BAF.Scheduler.Specifications;
using Ninject.Modules;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;

namespace BAF.Scheduler
{
    public class NinjectRegistration : NinjectModule
    {
        public override void Load()
        {
            Bind<IJobFactory>().To<JobFactory>().InSingletonScope();
            Bind<ISchedulerFactory>().To<SchedulerFactory>().InSingletonScope();
            Bind<ISchedulerLogger>().To<SchedulerDbLogger>().InSingletonScope();
            Bind<IScheduler, IBAFScheduler>().ToProvider<SchedulerProvider>().InSingletonScope();
            Bind<ISchedulerFacade>().To<SchedulerFacade>().InSingletonScope();
        }
    }
}
