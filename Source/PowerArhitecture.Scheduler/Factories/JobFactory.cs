using System;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Syntax;
using Quartz;
using Quartz.Spi;

namespace BAF.Scheduler.Factories
{
    public class JobFactory : IJobFactory
    {
        private readonly IResolutionRoot _resolutionRoot;
        private readonly ILogger _logger;

        public JobFactory(IResolutionRoot resolutionRoot, ILogger logger)
        {
            _resolutionRoot = resolutionRoot;
            _logger = logger;
        }

        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            try
            {   
                var jobDetail = bundle.JobDetail;
                var jobType = jobDetail.JobType;
                _logger.Debug("Instantiating job of type " + jobType.FullName);
                return (IJob)_resolutionRoot.Get(jobType);
            }
            catch (Exception e)
            {
                _logger.Fatal("Problem instantiating job {0}", e);
                throw new SchedulerException("Problem instantiating job", e);
            }
        }

        public void ReturnJob(IJob job)
        {
            _logger.Debug("ReturnJob call for job " + job.GetType().FullName);
        }
    }
}
