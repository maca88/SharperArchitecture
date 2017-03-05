using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Breeze.ContextProvider;

namespace PowerArhitecture.Breeze
{
    public class SaveInterceptorSettings
    {
        public Action<List<EntityInfo>> BeforeSave { get; set; }

        public Action<Dictionary<Type, List<EntityInfo>>, List<KeyMapping>> AfterSave { get; set; }

        public Action<List<EntityInfo>> AfterFlush { get; set; }

        public Action<List<EntityInfo>> BeforeFlush { get; set; }
    }
}
