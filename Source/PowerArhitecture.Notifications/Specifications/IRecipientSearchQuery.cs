using System.Collections.Generic;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Notifications.Specifications
{
    public interface IRecipientSearchQuery
    {
        //string QueryName { get; }

        IEnumerable<IUser> GetRecipients(string pattern);
    }
}