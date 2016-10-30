using MediatR;

namespace PowerArhitecture.Common.Specifications
{
    public interface IEvent : INotification, ICancellableAsyncNotification
    {
        
    }
}
