using System;
using System.Collections.Generic;
using System.Linq;
using PowerArhitecture.Common.Events;
using Bootstrap.Extensions.StartupTasks;
using Ninject;
using Ninject.Extensions.Logging;
using Ninject.Syntax;

namespace PowerArhitecture.Common.StartupTasks
{/*
    [Task(PositionInSequence = -1)] //Must be first
    public class AddListenersTask : IStartupTask
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IResolutionRoot _resolutionRoot;
        private readonly IEnumerable<Type> _listenerTypes;
        private readonly ILogger _logger;
        private readonly IList<dynamic> _listeners;

        public AddListenersTask(ILogger logger, IEventAggregator eventAggregator, IResolutionRoot resolutionRoot)
        {
            _listeners = new List<dynamic>();
            _logger = logger;
            _eventAggregator = eventAggregator;
            _resolutionRoot = resolutionRoot;
            _listenerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes()
                    .Where(o => !o.IsInterface && o.IsAssignableToGenericType(typeof(IListener<>)) && o != typeof(DelegateListener<>)))
                .ToList();
        }

        public void Run()
        {
            foreach (var listenerType in _listenerTypes)
            {
                var listener = _resolutionRoot.Get(listenerType);
                _logger.Info("Adding new listener '{0}'", listenerType.FullName);
                _listeners.Add(listener);
                _eventAggregator.AddListener(listener);
            }
        }

        public void Reset()
        {
            while (_listeners.Any())
            {
                var listener = _listeners[0];
                _logger.Info("Removing listener '{0}'", listener.GetType().FullName);
                _eventAggregator.RemoveListener(listener);
                _listeners.RemoveAt(0);
            }
        }
    }*/
}
