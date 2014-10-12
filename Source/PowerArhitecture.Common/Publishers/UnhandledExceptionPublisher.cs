using System;
using PowerArhitecture.Common.Events;

namespace PowerArhitecture.Common.Publishers
{
    public class UnhandledExceptionPublisher
    {
        private readonly IEventAggregator _eventAggregator;

        public UnhandledExceptionPublisher(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }

        private void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            _eventAggregator.SendMessage(new UnhandledExceptionEvent(e.ExceptionObject as Exception));
        }
    }
}
