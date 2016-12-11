﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Tests.Common.Commands
{
    public class TestAsyncCommand : IAsyncCommand<bool>
    {
    }

    public class TestAsyncCommandHandler : IAsyncCommandHandler<TestAsyncCommand, bool>
    {
        public async Task<bool> HandleAsync(TestAsyncCommand command, CancellationToken cancellationToken)
        {
            await Task.Yield();
            return true;
        }
    }
}
