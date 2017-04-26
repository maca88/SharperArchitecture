using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;
using SharperArchitecture.Common.Specifications;

namespace SharperArchitecture.Breeze.Events
{
    public class BreezeAfterSaveEvent : IEvent
    {
        public BreezeAfterSaveEvent(Dictionary<Type, List<EntityInfo>> saveMap, List<KeyMapping> keyMappings)
        {
            SaveMap = saveMap;
            KeyMappings = keyMappings;
        }

        public Dictionary<Type, List<EntityInfo>> SaveMap { get; }

        public List<KeyMapping> KeyMappings { get; }
    }
}
