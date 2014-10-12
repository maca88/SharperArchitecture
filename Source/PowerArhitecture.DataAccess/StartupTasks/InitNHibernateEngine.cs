using Bootstrap.Extensions.StartupTasks;
using Ninject.Syntax;

namespace PowerArhitecture.DataAccess.StartupTasks
{
    public class InitNHibernateEngine : IStartupTask
    {
        private readonly IResolutionRoot _resolutionRoot;

        public InitNHibernateEngine(IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
        }

        public void Run()
        {
        }

        public void Reset()
        {
            
        }
    }
}
