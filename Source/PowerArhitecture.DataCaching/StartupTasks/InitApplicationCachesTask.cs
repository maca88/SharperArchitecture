using System;
using PowerArhitecture.DataCaching.Specifications;
using Bootstrap.Extensions.StartupTasks;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Syntax;

namespace PowerArhitecture.DataCaching.StartupTasks
{
    [Task(PositionInSequence = 1000)] //Have to be executed after AddListenersTask because an app cache can have dependency to a class that can trigger an event
    public class InitApplicationCachesTask : IStartupTask
    {
        private readonly IResolutionRoot _resolutionRoot;
        private readonly ILogger _logger;
        
        public InitApplicationCachesTask(ILogger logger, IResolutionRoot resolutionRoot)
        {
            _resolutionRoot = resolutionRoot;
            _logger = logger;
        }

        public void Run()
        {
            var appCaches = _resolutionRoot.GetAll<IApplicationCache>(); //if app caches are injected in ctor all app cache dependencies that have to be initialized can trigger events before event listeners are added
            foreach (var appCache in appCaches)
            {
                var typeFullName = appCache.GetType().FullName;
                var baseAppCache = appCache as BaseApplicationCache;
                if (baseAppCache == null) throw new Exception(string.Format("Application cache '{0}' must inherit BaseApplicationCache", typeFullName));
                baseAppCache.BeforeInitialization();
                _logger.Info("Initializing ApplicationCache of type '{0}'...", typeFullName);
                appCache.Initialize();
                _logger.Info("ApplicationCache of type '{0}' Initialized", typeFullName);
                if (baseAppCache.IsMaster)
                {
                    _logger.Info("ApplicationCache of type '{0}' is master... start filling cache", typeFullName);
                    appCache.Refresh();
                    _logger.Info("filling cache completed", typeFullName);
                }
                baseAppCache.AfterInitialization();
            } 
        }

        public void Reset()
        {
        }
    }
}
