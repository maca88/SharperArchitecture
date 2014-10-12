using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAF.DataAccess.Specifications;
using BAF.Scheduler.Entities;

namespace BAF.Scheduler
{
    public class SchedulerDbLogger : SchedulerLoggerBase
    {
        private readonly IUnitOfWorkFactory _unitOfWorkFactory;

        public SchedulerDbLogger(IUnitOfWorkFactory unitOfWorkFactory, string schedulerName) : base(schedulerName)
        {
            _unitOfWorkFactory = unitOfWorkFactory;
        }

        public override void GetLogs(Action<IQueryable<SchedulerLog>> queryAction)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetNew())
            {
                var query = unitOfWork.GetRepository<SchedulerLog>().GetLinqQuery();
                queryAction(query);
            }
        }

        public override void Save(SchedulerLog log)
        {
            using (var unitOfWork = _unitOfWorkFactory.GetNew())
            {
                unitOfWork.Save(log);
            }
        }
    }
}
