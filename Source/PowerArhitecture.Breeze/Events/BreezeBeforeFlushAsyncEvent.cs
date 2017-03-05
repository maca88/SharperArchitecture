﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using PowerArhitecture.Common.Specifications;

namespace PowerArhitecture.Breeze.Events
{
    public class BreezeBeforeFlushAsyncEvent : IAsyncEvent
    {
        public BreezeBeforeFlushAsyncEvent(List<EntityInfo> entitiesToPersist)
        {
            EntitiesToPersist = entitiesToPersist;
        }

        public List<EntityInfo> EntitiesToPersist { get; }
    }
}
