using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Breeze.Events
{
    public class BreezeBeforeSaveAsyncEvent : IAsyncEvent
    {
        public BreezeBeforeSaveAsyncEvent(List<EntityInfo> entitiesToPersist)
        {
            EntitiesToPersist = entitiesToPersist;
        }

        public List<EntityInfo> EntitiesToPersist { get; }
    }
}
