using System;
using SharperArchitecture.Common.Events;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Common.Publishers
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
