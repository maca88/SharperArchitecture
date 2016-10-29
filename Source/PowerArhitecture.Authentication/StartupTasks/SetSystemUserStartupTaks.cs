using PowerArhitecture.Authentication.Entities;
using PowerArhitecture.Common.Helpers;
using PowerArhitecture.DataAccess.Specifications;
using PowerArhitecture.Authentication.Specifications;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Syntax;

namespace PowerArhitecture.Authentication.StartupTasks
{
    /*
    public class SetSystemUserStartupTaks : IStartupTask
    {
        private readonly IResolutionRoot _resolutionRoot;
        private readonly ILogger _logger;

        public SetSystemUserStartupTaks(ILogger logger, IResolutionRoot resolutionRoot)
        {
            _logger = logger;
            _resolutionRoot = resolutionRoot;
        }

        public void Run()
        {
            var authSettings = _resolutionRoot.Get<IAuthenticationSettings>();
            using (var unitOfWork = _resolutionRoot.Get<IUnitOfWorkFactory>().GetNew())
            {
                var systemUser = unitOfWork.GetEntitiesQuery<User>()
                                           .Include(o => o.Roles)
                                           .SingleOrDefault(o => o.UserName == authSettings.SystemUserName);
                //PrincipalHelper.SetSystemUser(unitOfWork.DeepCopy(systemUser));
            }
        }

        public void Reset()
        {
            
        }
    }*/
}
