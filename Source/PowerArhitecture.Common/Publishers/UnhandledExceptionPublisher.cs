using System;
using PowerArhitecture.Common.Events;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Common.Publishers
{
    public class UnhandledExceptionPublisher
    {
        private readonly IEventPublisher _eventPublisher;

        public UnhandledExceptionPublisher(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
            AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionTrapper;
        }

        private void UnhandledExceptionTrapper(object sender, UnhandledExceptionEventArgs e)
        {
            _eventPublisher.Publish(new UnhandledExceptionEvent(e.ExceptionObject as Exception));
        }
    }
}
