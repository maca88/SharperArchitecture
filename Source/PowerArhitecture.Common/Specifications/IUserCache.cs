﻿using System;
using System.Security.Principal;

namespace PowerArhitecture.Common.Specifications
{
    public interface IUserCache
    {
        IUser GetCurrentUser();

        IPrincipal GetCurrentPrincipal();

        IUser GetUser(string userName);

        DateTimeOffset GetDateTimeOffset(DateTime userDateTime);

    }
}